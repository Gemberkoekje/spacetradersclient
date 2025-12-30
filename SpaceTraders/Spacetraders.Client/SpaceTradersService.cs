using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace SpaceTraders.Client;

/// <summary>
/// Provides a rate-limited service for executing SpaceTraders API operations.
/// </summary>
public sealed class SpaceTradersService(
    SpaceTradersClient client,
    int permitsPerSecond = 2,
    int burst = 30,
    int capacity = 1024,
    int burstDurationSeconds = 60
) : IAsyncDisposable
{
    private interface IWorkItem
    {
        ValueTask ExecuteAsync(SpaceTradersClient client, CancellationToken ct);

        void TryCancel(CancellationToken ct);
    }

    private sealed class WorkItem<T>(Func<SpaceTradersClient, CancellationToken, Task<T>> op, TaskCompletionSource<Result<T>> tcs) : IWorkItem
    {
        public async ValueTask ExecuteAsync(SpaceTradersClient client, CancellationToken ct)
        {
            try
            {
                var result = await op(client, ct).ConfigureAwait(false);
                tcs.TrySetResult(result);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                tcs.TrySetCanceled(ct);
            }
            catch (ApiException ex)
            {
                // If we hit a 429 (Too Many Requests), wait a bit and retry once.
                if (ex.StatusCode == 429)
                {
                    // Try read Retry-After from response headers if available, else default to 5 seconds.
                    var retryAfter = TimeSpan.FromSeconds(5);
                    try
                    {
                        if (ex.Headers is not null && ex.Headers.TryGetValue("Retry-After", out var values))
                        {
                            var header = values?.FirstOrDefault();
                            if (int.TryParse(header, out var seconds) && seconds > 0)
                            {
                                retryAfter = TimeSpan.FromSeconds(seconds);
                            }
                        }
                    }
                    catch
                    {
                        // ignore header parsing issues
                    }

                    try
                    {
                        await Task.Delay(retryAfter, ct).ConfigureAwait(false);
                        var retryResult = await op(client, ct).ConfigureAwait(false);
                        tcs.TrySetResult(retryResult);
                        return;
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(ct);
                        return;
                    }
                    catch (ApiException retryEx) when (retryEx.StatusCode == 429)
                    {
                        // Fall through to normal error handling below
                    }
                    catch (Exception retryEx)
                    {
                        tcs.TrySetException(retryEx);
                        return;
                    }
                }

                var response = ex.Response;
                var error = JsonSerializer.Deserialize<ErrorResponse>(response);
                if (error is null || error.error is null)
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to deserialize error response.", ex));
                    return;
                }

                tcs.TrySetResult(Result.WithMessages<T>(ValidationMessage.Error(error.error.message)));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        public void TryCancel(CancellationToken ct) => tcs.TrySetCanceled(ct);
    }

    private readonly Channel<IWorkItem> _priorityQueue = Channel.CreateBounded<IWorkItem>(
        new BoundedChannelOptions(capacity) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.Wait });

    private readonly Channel<IWorkItem> _queue = Channel.CreateBounded<IWorkItem>(
        new BoundedChannelOptions(capacity) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.Wait });

    private readonly CancellationTokenSource _cts = new ();
    private readonly RateLimiter _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
    {
        TokenLimit = Math.Max(1, burst),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0,
        ReplenishmentPeriod = TimeSpan.FromSeconds(Math.Max(1, burstDurationSeconds)),
        TokensPerPeriod = Math.Max(1, permitsPerSecond * Math.Max(1, burstDurationSeconds)),
        AutoReplenishment = true,
    });

    private int _started;
    private Task? _processingLoop;

    /// <summary>
    /// Enqueues an operation to be executed against the SpaceTraders API.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="priority">Whether this is a priority operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the result of the operation.</returns>
    public Task<Result<T>> EnqueueAsync<T>(
        Func<SpaceTradersClient, CancellationToken, Task<T>> operation,
        bool priority = false,
        CancellationToken cancellationToken = default)
    {
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        EnsureStarted();

        var tcs = new TaskCompletionSource<Result<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var item = new WorkItem<T>(operation, tcs);

        var writer = priority ? _priorityQueue.Writer : _queue.Writer;
        _ = writer.WriteAsync(item, cancellationToken);

        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() => item.TryCancel(cancellationToken));
        }

        return tcs.Task;
    }

    private void EnsureStarted()
    {
        if (Interlocked.Exchange(ref _started, 1) == 0)
        {
            _processingLoop = Task.Run(ProcessLoopAsync);
        }
    }

    private async Task ProcessLoopAsync()
    {
        var high = _priorityQueue.Reader;
        var low = _queue.Reader;
        var ct = _cts.Token;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                // Prefer priority work if available
                if (!high.TryRead(out var work) && !low.TryRead(out work))
                {
                    // Wait until any queue has data
                    var highWait = high.WaitToReadAsync(ct).AsTask();
                    var lowWait = low.WaitToReadAsync(ct).AsTask();
                    var winner = await Task.WhenAny(highWait, lowWait).ConfigureAwait(false);
                    if (!await winner.ConfigureAwait(false))
                    {
                        break;
                    }

                    // Try again to read
                    if (!high.TryRead(out work) && !low.TryRead(out work))
                    {
                        continue;
                    }
                }

                // Acquire a token before executing the dequeued work. If none available, wait according to RetryAfter.
                while (true)
                {
                    using var lease = await _limiter.AcquireAsync(1, ct).ConfigureAwait(false);
                    if (lease.IsAcquired)
                    {
                        break;
                    }

                    // Try to get suggested retry delay and wait. If absent, wait a minimal delay to avoid a tight loop.
                    if (lease.TryGetMetadata("RetryAfter", out object? retryObj) && retryObj is TimeSpan retryAfter && retryAfter > TimeSpan.Zero)
                    {
                        await Task.Delay(retryAfter, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100), ct).ConfigureAwait(false);
                    }
                }

                await work.ExecuteAsync(client, ct).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // graceful shutdown
        }
        finally
        {
            // Cancel anything still queued
            while (high.TryRead(out var w))
            {
                w.TryCancel(ct);
            }

            while (low.TryRead(out var w))
            {
                w.TryCancel(ct);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync().ConfigureAwait(false);
        _priorityQueue.Writer.TryComplete();
        _queue.Writer.TryComplete();

        if (_processingLoop is not null)
        {
            try
            {
                await _processingLoop.ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }
        }

        await _limiter.DisposeAsync().ConfigureAwait(false);
        _cts.Dispose();
    }
}
