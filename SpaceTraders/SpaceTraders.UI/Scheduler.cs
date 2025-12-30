using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

/// <summary>
/// A scheduler for executing tasks at specific times.
/// </summary>
public sealed class Scheduler : IDisposable
{
    private readonly object _lock = new ();

    private readonly PriorityQueue<(DateTimeOffset runAt, Func<Task> action), DateTimeOffset> _pq = new ();

    private Timer? _timer;

    /// <summary>
    /// Enqueues an action to be executed at a specific time.
    /// </summary>
    /// <param name="when">The time at which to execute the action.</param>
    /// <param name="action">The action to execute.</param>
    public void Enqueue(DateTimeOffset when, Func<Task> action)
    {
        lock (_lock)
        {
            _pq.Enqueue((when, action), when);
            ScheduleNextLocked();
        }
    }

    private void ScheduleNextLocked()
    {
        _timer?.Dispose();
        if (_pq.Count == 0) return;
        var next = _pq.Peek().runAt;
        var dueIn = next - TimeProvider.System.GetUtcNow();
        if (dueIn < TimeSpan.Zero) dueIn = TimeSpan.Zero;
        _timer = new Timer(_ => RunDue().ConfigureAwait(false), null, dueIn, Timeout.InfiniteTimeSpan);
    }

    private async Task RunDue()
    {
        List<Func<Task>> due = [];
        lock (_lock)
        {
            var now = TimeProvider.System.GetUtcNow();
            while (_pq.Count > 0 && _pq.Peek().runAt <= now)
            {
                var item = _pq.Dequeue();
                due.Add(item.action);
            }
            ScheduleNextLocked();
        }
        foreach (var act in due)
        {
            try { await act(); } catch { /* log */ }
        }
    }

    /// <summary>
    /// Disposes of the scheduler and releases resources.
    /// </summary>
    public void Dispose() => _timer?.Dispose();
}
