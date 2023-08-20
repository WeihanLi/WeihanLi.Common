using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WeihanLi.Common.Helpers;

public static class InvokeHelper
{
    static InvokeHelper()
    {
        OnInvokeException = ex => Debug.WriteLine(ex);

        #region Exit event register

        // https://newlifex.com/blood/elegant_exit
        // https://github.com/NewLifeX/X/blob/e65dfa0998ec393804f3f793f333c237110d890e/NewLife.Core/Model/Host.cs#L61
        // https://github.com/dotnet/runtime/blob/940b332ad04e58862febe019788a5b21e266ea10/src/libraries/Microsoft.Extensions.Hosting/src/Internal/ConsoleLifetime.notnetcoreapp.cs
        AppDomain.CurrentDomain.ProcessExit += InvokeExitHandler;
        Console.CancelKeyPress += InvokeExitHandler;
#if NETCOREAPP
        System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += ctx => InvokeExitHandler(ctx, null);
#endif
#if NET6_0_OR_GREATER
        // https://github.com/dotnet/runtime/blob/940b332ad04e58862febe019788a5b21e266ea10/src/libraries/Microsoft.Extensions.Hosting/src/Internal/ConsoleLifetime.netcoreapp.cs
        PosixSignalRegistration.Create(PosixSignal.SIGINT, ctx => InvokeExitHandler(ctx, null));
        PosixSignalRegistration.Create(PosixSignal.SIGQUIT, ctx => InvokeExitHandler(ctx, null));
        PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx => InvokeExitHandler(ctx, null));
#endif

        #endregion ExitHandler
    }

    #region Profile

    public static double Profile(Action action)
    {
        Guard.NotNull(action, nameof(action));
        var stopwatch = ValueStopwatch.StartNew();
        action();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static double Profile<T>(Action<T> action, T t)
    {
        Guard.NotNull(action, nameof(action));
        var stopwatch = ValueStopwatch.StartNew();
        action(t);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static double Profile<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
    {
        Guard.NotNull(action, nameof(action));
        var stopwatch = ValueStopwatch.StartNew();
        action(t1, t2);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static double Profile<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
    {
        var stopwatch = ValueStopwatch.StartNew();
        action(t1, t2, t3);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static async Task<double> ProfileAsync(Func<Task> action)
    {
        var stopwatch = ValueStopwatch.StartNew();
        await action();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static async Task<double> ProfileAsync<T>(Func<T, Task> func, T t)
    {
        var stopwatch = ValueStopwatch.StartNew();
        await func(t);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static async Task<double> ProfileAsync<T1, T2>(Func<T1, T2, Task> func, T1 t1, T2 t2)
    {
        var stopwatch = ValueStopwatch.StartNew();
        await func(t1, t2);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    public static async Task<double> ProfileAsync<T1, T2, T3>(Func<T1, T2, T3, Task> func, T1 t1, T2 t2, T3 t3)
    {
        var stopwatch = ValueStopwatch.StartNew();
        await func(t1, t2, t3);
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    #endregion Profile

    #region TryInvoke

    public static Action<Exception>? OnInvokeException { get; set; }

    private static readonly object _exitLock = new();
    private static volatile bool _exited;
    private static readonly Lazy<CancellationTokenSource> LazyCancellationTokenSource = new();
    private static void InvokeExitHandler(object? sender, EventArgs? args)
    {
        if (_exited) return;
        lock (_exitLock)
        {
            if (_exited) return;

            Debug.WriteLine("exiting...");
            if (LazyCancellationTokenSource.IsValueCreated)
            {
                LazyCancellationTokenSource.Value.Cancel();
                LazyCancellationTokenSource.Value.Dispose();
            }
            Debug.WriteLine("exited");
            _exited = true;
        }
    }

    public static CancellationToken GetExitToken() => LazyCancellationTokenSource.Value.Token;

    public static void TryInvoke(Action action)
    {
        Guard.NotNull(action, nameof(action));
        try
        {
            action();
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static void TryInvoke<T>(Action<T> action, T t)
    {
        Guard.NotNull(action, nameof(action));
        try
        {
            action(t);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static void TryInvoke<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
    {
        Guard.NotNull(action, nameof(action));
        try
        {
            action(t1, t2);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static async Task TryInvokeAsync<T1, T2>(Func<T1, T2, Task> func, T1 t1, T2 t2)
    {
        Guard.NotNull(func, nameof(func));
        try
        {
            await func(t1, t2);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static void TryInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
    {
        Guard.NotNull(action, nameof(action));
        try
        {
            action(t1, t2, t3);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static async Task TryInvokeAsync(Func<Task> func)
    {
        Guard.NotNull(func, nameof(func));
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static async Task TryInvokeAsync<T>(Func<T, Task> func, T t)
    {
        Guard.NotNull(func, nameof(func));
        try
        {
            await func(t);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    public static async Task TryInvokeAsync<T1, T2, T3>(Func<T1, T2, T3, Task> func, T1 t1, T2 t2, T3 t3)
    {
        Guard.NotNull(func, nameof(func));

        try
        {
            await func(t1, t2, t3);
        }
        catch (Exception ex)
        {
            OnInvokeException?.Invoke(ex);
        }
    }

    #endregion TryInvoke
}
