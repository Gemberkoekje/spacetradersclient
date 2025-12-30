using System;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Extensions;

/// <summary>
/// Helper to safely discard tasks (fire-and-forget) while observing and handling exceptions.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Safely fires and forgets a task, optionally handling exceptions.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="onError">Optional error handler.</param>
    public static void SafeFireAndForget(this Task task, Action<Exception>? onError = null)
    {
        if (task is null) return;

        if (task.IsCompleted)
        {
            if (task.IsFaulted)
                onError?.Invoke(task.Exception!.GetBaseException());
            return;
        }

        _ = ForgetAwaited(task, onError);

        static async Task ForgetAwaited(Task task, Action<Exception>? onError)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}
