using System;
using System.Threading;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace WeihanLi.Common.Helpers;

public static class RetryHelper
{
    #region TryInvoke

    public static bool TryInvoke(Action action, int maxRetryTimes = 3, Action<int, TimeSpan, Exception?>? onRetry = null, Func<int, TimeSpan>? delayFunc = null)
    {
        Guard.NotNull(action, nameof(action));

        var time = 0;
        do
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                time++;
                var delay = delayFunc?.Invoke(time);
                onRetry?.Invoke(time, delay.GetValueOrDefault(), ex);
                if (delay.HasValue)
                {
                    Thread.Sleep(delay.Value);
                }
            }
        } while (time <= maxRetryTimes);
        return false;
    }

    public static bool TryInvoke(Func<bool> func, int maxRetryTimes = 3, Action<int, TimeSpan, Exception?>? onRetry = null, Func<int, TimeSpan>? delayFunc = null)
    {
        Guard.NotNull(func, nameof(func));

        var result = false;
        var time = 0;
        var exception = default(Exception);
        do
        {
            if (time > 0)
            {
                var delay = delayFunc?.Invoke(time);
                onRetry?.Invoke(time, delay.GetValueOrDefault(), exception);
                if (delay.HasValue)
                {
                    Task.Delay(delay.Value).Wait();
                }
            }
            try
            {
                result = func();
                exception = null;
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
                exception = ex;
            }

            time++;
        } while (!result && time <= maxRetryTimes);
        return result;
    }

    public static TResult? TryInvoke<TResult>(Func<TResult?> func, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (time > 0 && delayFunc != null)
            {
                Task.Delay(delayFunc.Invoke(time)).Wait();
            }
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static TResult? TryInvoke<T1, TResult>(Func<T1, TResult?> func, T1 t1, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (time > 0 && delayFunc != null)
            {
                Task.Delay(delayFunc.Invoke(time)).Wait();
            }
            try
            {
                result = func(t1);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);

        return result;
    }

    public static TResult? TryInvoke<T1, T2, TResult>(Func<T1, T2, TResult?> func, T1 t1, T2 t2, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (time > 0 && delayFunc != null)
            {
                Task.Delay(delayFunc.Invoke(time)).Wait();
            }
            try
            {
                result = func(t1, t2);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static TResult? TryInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult?> func, T1 t1, T2 t2, T3 t3, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (time > 0 && delayFunc != null)
            {
                Task.Delay(delayFunc.Invoke(time)).Wait();
            }

            try
            {
                result = func(t1, t2, t3);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static TResult TryInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult)!;
        var time = 0;
        do
        {
            if (time > 0 && delayFunc != null)
            {
                Task.Delay(delayFunc.Invoke(time)).Wait();
            }
            try
            {
                result = func(t1, t2, t3, t4);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    #endregion TryInvoke

    #region TryInvokeAsync

    public static async Task<bool> TryInvokeAsync(Func<Task> action, int maxRetryTimes = 3, Action<int, TimeSpan, Exception>? onRetry = null, Func<int, TimeSpan>? delayFunc = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(action, nameof(action));

        var time = 0;
        do
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                time++;
                var delay = delayFunc?.Invoke(time);
                onRetry?.Invoke(time, delay.GetValueOrDefault(), ex);
                if (delay.HasValue)
                {
                    await Task.Delay(delay.Value, cancellationToken);
                }
            }
        } while (time <= maxRetryTimes && !cancellationToken.IsCancellationRequested);
        return false;
    }

    public static async Task<bool> TryInvokeAsync(Func<Task<bool>> func, int maxRetryTimes = 3, Action<int, TimeSpan, Exception?>? onRetry = null, Func<int, TimeSpan>? delayFunc = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(func, nameof(func));

        var result = false;
        var time = 0;
        var exception = default(Exception);
        do
        {
            if (time > 0)
            {
                var delay = delayFunc?.Invoke(time);
                onRetry?.Invoke(time, delay.GetValueOrDefault(), exception);
                if (delay.HasValue)
                {
                    await Task.Delay(delay.Value, cancellationToken);
                }
            }
            try
            {
                result = await func();
                exception = null;
            }
            catch (Exception ex)
            {
                exception = ex;
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
            time++;
        } while (!result && time <= maxRetryTimes && !cancellationToken.IsCancellationRequested);

        return result;
    }

    public static async Task<TResult?> TryInvokeAsync<TResult>(Func<Task<TResult?>> func, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (delayFunc != null && time > 0)
            {
                await Task.Delay(delayFunc(time));
            }

            try
            {
                result = await func();
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static async Task<TResult?> TryInvokeAsync<T1, TResult>(Func<T1, Task<TResult?>> func, T1 t1, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (delayFunc != null && time > 0)
            {
                await Task.Delay(delayFunc(time));
            }
            try
            {
                result = await func(t1);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static async Task<TResult?> TryInvokeAsync<T1, T2, TResult>(Func<T1, T2, Task<TResult?>> func, T1 t1, T2 t2, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (delayFunc != null && time > 0)
            {
                await Task.Delay(delayFunc(time));
            }
            try
            {
                result = await func(t1, t2);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static async Task<TResult?> TryInvokeAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult?>> func, T1 t1, T2 t2, T3 t3, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (delayFunc != null && time > 0)
            {
                await Task.Delay(delayFunc(time));
            }
            try
            {
                result = await func(t1, t2, t3);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    public static async Task<TResult?> TryInvokeAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult?, bool> validFunc, int maxRetryTimes = 3, Func<int, TimeSpan>? delayFunc = null)
    {
        var result = default(TResult);
        var time = 0;
        do
        {
            if (delayFunc != null && time > 0)
            {
                await Task.Delay(delayFunc(time));
            }
            try
            {
                result = await func(t1, t2, t3, t4);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }

            time++;
        } while (!validFunc(result) && time <= maxRetryTimes);
        return result;
    }

    #endregion TryInvokeAsync
}
