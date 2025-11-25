using SpaceTraders.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Spacetraders.Client;

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

    private sealed class WorkItem<T>(Func<SpaceTradersClient, CancellationToken, Task<T>> op, TaskCompletionSource<T> tcs) : IWorkItem
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
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        public void TryCancel(CancellationToken ct) => tcs.TrySetCanceled(ct);
    }

    private sealed class WorkItem(Func<SpaceTradersClient, CancellationToken, Task> op, TaskCompletionSource tcs) : IWorkItem
    {
        public async ValueTask ExecuteAsync(SpaceTradersClient client, CancellationToken ct)
        {
            try
            {
                await op(client, ct).ConfigureAwait(false);
                tcs.TrySetResult();
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                tcs.TrySetCanceled(ct);
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

    private readonly CancellationTokenSource _cts = new();
    private readonly RateLimiter _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
    {
        TokenLimit = Math.Max(1, burst),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0, // we do our own queuing via Channels
        ReplenishmentPeriod = TimeSpan.FromSeconds(Math.Max(1, burstDurationSeconds)),
        TokensPerPeriod = Math.Max(1, permitsPerSecond * Math.Max(1, burstDurationSeconds)),
        AutoReplenishment = true
    });

    private int _started;
    private Task? _processingLoop;

    // Simple in-memory cache for results
    private sealed class CacheItem(object value, Type type, DateTimeOffset expiresAt)
    {
        public object Value { get; } = value;
        public Type Type { get; } = type;
        public DateTimeOffset ExpiresAt { get; } = expiresAt;
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    }

    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();

    public Task<T> EnqueueAsync<T>(
        Func<SpaceTradersClient, CancellationToken, Task<T>> operation,
        bool priority = false,
        CancellationToken cancellationToken = default)
    {
        if (operation is null) throw new ArgumentNullException(nameof(operation));
        EnsureStarted();

        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var item = new WorkItem<T>(operation, tcs);

        var writer = priority ? _priorityQueue.Writer : _queue.Writer;
        _ = writer.WriteAsync(item, cancellationToken);

        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() => item.TryCancel(cancellationToken));
        }

        return tcs.Task;
    }

    // Cached variant: returns cached result by cacheKey until ttl expires.
    public Task<T> EnqueueCachedAsync<T>(
        Func<SpaceTradersClient, CancellationToken, Task<T>> operation,
        string cacheKey,
        TimeSpan ttl,
        bool priority = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey) || ttl <= TimeSpan.Zero)
        {
            return EnqueueAsync(operation, priority, cancellationToken);
        }

        if (_cache.TryGetValue(cacheKey, out var item))
        {
            if (!item.IsExpired && item.Type == typeof(T))
            {
                return Task.FromResult((T)item.Value);
            }
            else
            {
                _cache.TryRemove(cacheKey, out _);
            }
        }

        var task = EnqueueAsync(operation, priority, cancellationToken);
        _ = task.ContinueWith(t =>
        {
            if (t.Status == TaskStatus.RanToCompletion)
            {
                var expires = DateTimeOffset.UtcNow.Add(ttl);
                _cache[cacheKey] = new CacheItem(t.Result!, typeof(T), expires);
            }
        }, TaskScheduler.Default);
        return task;
    }

    public Task EnqueueAsync(
        Func<SpaceTradersClient, CancellationToken, Task> operation,
        bool priority = false,
        CancellationToken cancellationToken = default)
    {
        if (operation is null) throw new ArgumentNullException(nameof(operation));
        EnsureStarted();

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var item = new WorkItem(operation, tcs);

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
                    if (!await winner.ConfigureAwait(false)) break;

                    // Try again to read
                    if (!high.TryRead(out work) && !low.TryRead(out work))
                        continue;
                }

                using var lease = await _limiter.AcquireAsync(1, ct).ConfigureAwait(false);
                if (!lease.IsAcquired)
                {
                    // Should not happen with TokenBucketRateLimiter; yield to avoid tight loop
                    await Task.Yield();
                    continue;
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
            while (high.TryRead(out var w)) w.TryCancel(ct);
            while (low.TryRead(out var w)) w.TryCancel(ct);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _priorityQueue.Writer.TryComplete();
        _queue.Writer.TryComplete();

        if (_processingLoop is not null)
        {
            try { await _processingLoop.ConfigureAwait(false); } catch { /* ignore */ }
        }

        _limiter.Dispose();
        _cts.Dispose();

        // Clear cache on dispose
        _cache.Clear();
    }
}