using System.Diagnostics;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class TaskExtension
{
    public static Task<T> WrapTask<T>(this T t) => Task.FromResult(t);

#if ValueTaskSupport
    public static ValueTask<T> WrapValueTask<T>(this T t) => new ValueTask<T>(t);
#endif

    public static Task AsTask(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<object?>();
        cancellationToken.Register(() =>
        {
            tcs.TrySetResult(null);
        });
        return tcs.Task;
    }

    public static Task WhenAny(this IEnumerable<Task> tasks) => Task.WhenAny(tasks);

    public static Task<Task<TResult>> WhenAny<TResult>(this IEnumerable<Task<TResult>> tasks) => Task.WhenAny(tasks);

    public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    public static Task WhenAllSafely(this IEnumerable<Task> tasks, Action<Exception>? onException = null) => Task.WhenAll(tasks.Select(async t =>
    {
        try
        {
            await t;
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
        }
    }));

    public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> tasks) => Task.WhenAll(tasks);

    #region TaskScheduler

    /// <summary>
    /// Runs an action on the current scheduler instead of the default scheduler.
    /// </summary>
    /// <param name="scheduler">Scheduler for the action to be scheduled on.</param>
    /// <param name="action">Action to be scheduled.</param>
    /// <param name="cancellationToken">Cancellation token to link the new task to. If canceled before being scheduled, the action will not be run.</param>
    /// <returns>New task created for the action.</returns>
    public static Task Run(this TaskScheduler scheduler, Action action, CancellationToken cancellationToken = default)
    {
        var taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, scheduler);
        return taskFactory.StartNew(action, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Runs a function on the current scheduler instead of the default scheduler.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="scheduler">Scheduler for the action to be scheduled on.</param>
    /// <param name="function">Function to be scheduled.</param>
    /// <param name="cancellationToken">Cancellation token to link the new task to. If canceled before being scheduled, the action will not be run.</param>
    /// <returns>New task created for the function. This task completes with the result of calling the function.</returns>
    public static Task<T> Run<T>(this TaskScheduler scheduler, Func<T> function, CancellationToken cancellationToken = default)
    {
        var taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, scheduler);
        return taskFactory.StartNew(function, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Runs a function on the current scheduler instead of the default scheduler.
    /// </summary>
    /// <param name="scheduler">Scheduler for the action to be scheduled on.</param>
    /// <param name="function">Function to be scheduled.</param>
    /// <param name="cancelationToken">Cancellation token to link the new task to. If canceled before being scheduled, the action will not be run.</param>
    /// <returns>New task created for the function. This task completes with the result of the task returned by the function.</returns>
    public static async Task Run(this TaskScheduler scheduler, Func<Task> function, CancellationToken cancelationToken = default)
    {
        var taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, scheduler);
        var innerTask = await taskFactory.StartNew(function, cancellationToken: cancelationToken);
        await innerTask;
    }

    /// <summary>
    /// Runs a function on the current scheduler instead of the default scheduler.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="scheduler">Scheduler for the action to be scheduled on.</param>
    /// <param name="function">Function to be scheduled.</param>
    /// <param name="cancelationToken">Cancellation token to link the new task to. If canceled before being scheduled, the action will not be run.</param>
    /// <returns>New task created for the function. This task completes with the result of the task returned by the function.</returns>
    public static async Task<T> Run<T>(this TaskScheduler scheduler, Func<Task<T>> function, CancellationToken cancelationToken = default)
    {
        var taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, scheduler);
        var innerTask = await taskFactory.StartNew(function, cancellationToken: cancelationToken);
        return await innerTask;
    }

    #endregion TaskScheduler

    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout,
[CallerFilePath] string? filePath = null,
[CallerLineNumber] int lineNumber = default)
    {
        if (task.IsCompleted || Debugger.IsAttached)
        {
            return await task;
        }

        using var cts = new CancellationTokenSource();
        if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
        {
            cts.Cancel();
            return await task;
        }

        throw new TimeoutException(CreateMessage(timeout, filePath, lineNumber));
    }

    public static async Task TimeoutAfter(this Task task, TimeSpan timeout,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = default)
    {
        if (task.IsCompleted || Debugger.IsAttached)
        {
            await task;
            return;
        }

        using var cts = new CancellationTokenSource();
        if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
        {
            cts.Cancel();
            await task;
        }
        else
        {
            throw new TimeoutException(CreateMessage(timeout, filePath, lineNumber));
        }
    }

    private static string CreateMessage(TimeSpan timeout, string? filePath, int lineNumber)
        => string.IsNullOrEmpty(filePath)
        ? $"The operation timed out after reaching the limit of {timeout.TotalMilliseconds}ms."
        : $"The operation at {filePath}:{lineNumber} timed out after reaching the limit of {timeout.TotalMilliseconds}ms.";
}
