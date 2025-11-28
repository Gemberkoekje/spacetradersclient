using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.UI;

public sealed class Scheduler : IDisposable
{
    private readonly object _lock = new(); private readonly PriorityQueue<(DateTimeOffset runAt, Func<Task> action), DateTimeOffset> _pq = new(); private Timer? _timer;
    public void Enqueue(DateTimeOffset when, Func<Task> action)
    {
        lock (_lock)
        {
            _pq.Enqueue((when, action), when);
            ScheduleNext_locked();
        }
    }

    private void ScheduleNext_locked()
    {
        _timer?.Dispose();
        if (_pq.Count == 0) return;
        var next = _pq.Peek().runAt;
        var dueIn = next - DateTimeOffset.UtcNow;
        if (dueIn < TimeSpan.Zero) dueIn = TimeSpan.Zero;
        _timer = new Timer(_ => RunDue(), null, dueIn, Timeout.InfiniteTimeSpan);
    }

    private async void RunDue()
    {
        List<Func<Task>> due = [];
        lock (_lock)
        {
            var now = DateTimeOffset.UtcNow;
            while (_pq.Count > 0 && _pq.Peek().runAt <= now)
            {
                var item = _pq.Dequeue();
                due.Add(item.action);
            }
            ScheduleNext_locked();
        }
        foreach (var act in due)
        {
            try { await act(); } catch { /* log */ }
        }
    }

    public void Dispose() => _timer?.Dispose();
}