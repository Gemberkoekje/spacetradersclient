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
                await HandleApiExceptionAsync(ex, client, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        private async ValueTask HandleApiExceptionAsync(ApiException ex, SpaceTradersClient client, CancellationToken ct)
        {
            if (ex.StatusCode == 429 && await TryRetryAfterRateLimitAsync(ex, client, ct).ConfigureAwait(false))
            {
                return;
            }

            HandleApiError(ex);
        }

        private async Task<bool> TryRetryAfterRateLimitAsync(ApiException ex, SpaceTradersClient client, CancellationToken ct)
        {
            var retryAfter = GetRetryAfterDelay(ex);

            try
            {
                await Task.Delay(retryAfter, ct).ConfigureAwait(false);
                var retryResult = await op(client, ct).ConfigureAwait(false);
                tcs.TrySetResult(retryResult);
                return true;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                tcs.TrySetCanceled(ct);
                return true;
            }
            catch (ApiException retryEx) when (retryEx.StatusCode == 429)
            {
                // Fall through to normal error handling
                return false;
            }
            catch (Exception retryEx)
            {
                tcs.TrySetException(retryEx);
                return true;
            }
        }

        private static TimeSpan GetRetryAfterDelay(ApiException ex)
        {
            try
            {
                if (ex.Headers is not null && ex.Headers.TryGetValue("Retry-After", out var values))
                {
                    var header = values?.FirstOrDefault();
                    if (int.TryParse(header, out var seconds) && seconds > 0)
                    {
                        return TimeSpan.FromSeconds(seconds);
                    }
                }
            }
            catch
            {
                // ignore header parsing issues
            }

            return TimeSpan.FromSeconds(5);
        }

        private void HandleApiError(ApiException ex)
        {
            var response = ex.Response;
            var error = JsonSerializer.Deserialize<ErrorResponse>(response);

            if (error?.error is null)
            {
                tcs.TrySetException(new InvalidOperationException("Failed to deserialize error response.", ex));
                return;
            }

            tcs.TrySetResult(Result.WithMessages<T>(ValidationMessage.Error(error.error.message)));
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
        var ct = _cts.Token;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var work = await DequeueWorkItemAsync(ct).ConfigureAwait(false);
                if (work is null)
                {
                    break;
                }

                await AcquireRateLimitTokenAsync(ct).ConfigureAwait(false);
                await work.ExecuteAsync(client, ct).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // graceful shutdown
        }
        finally
        {
            DrainQueues(ct);
        }
    }

    private async Task<IWorkItem?> DequeueWorkItemAsync(CancellationToken ct)
    {
        var high = _priorityQueue.Reader;
        var low = _queue.Reader;

        // Prefer priority work if available
        if (high.TryRead(out var work) || low.TryRead(out work))
        {
            return work;
        }

        // Wait until any queue has data
        var highWait = high.WaitToReadAsync(ct).AsTask();
        var lowWait = low.WaitToReadAsync(ct).AsTask();
        var winner = await Task.WhenAny(highWait, lowWait).ConfigureAwait(false);

        if (!await winner.ConfigureAwait(false))
        {
            return null;
        }

        // Try again to read, return null if still nothing available
        if (high.TryRead(out work) || low.TryRead(out work))
        {
            return work;
        }

        return null;
    }

    private async Task AcquireRateLimitTokenAsync(CancellationToken ct)
    {
        while (true)
        {
            using var lease = await _limiter.AcquireAsync(1, ct).ConfigureAwait(false);
            if (lease.IsAcquired)
            {
                return;
            }

            var delay = GetRetryDelay(lease);
            await Task.Delay(delay, ct).ConfigureAwait(false);
        }
    }

    private static TimeSpan GetRetryDelay(RateLimitLease lease)
    {
        if (lease.TryGetMetadata("RetryAfter", out object? retryObj) &&
            retryObj is TimeSpan retryAfter &&
            retryAfter > TimeSpan.Zero)
        {
            return retryAfter;
        }

        return TimeSpan.FromMilliseconds(100);
    }

    private void DrainQueues(CancellationToken ct)
    {
        while (_priorityQueue.Reader.TryRead(out var w))
        {
            w.TryCancel(ct);
        }

        while (_queue.Reader.TryRead(out var w))
        {
            w.TryCancel(ct);
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
