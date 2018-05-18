using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// 重试帮助类
    /// </summary>
    public static class RetryHelper
    {
        public static bool TryInvoke(Func<bool> func, int maxRetryTimes = 3)
        {
            var result = func();
            var time = 1;
            while (!result && time++ < maxRetryTimes)
            {
                result = func();
            }
            return result;
        }

        public static async Task<bool> TryInvokeAsync(Func<Task<bool>> func, int maxRetryTimes = 3)
        {
            var result = await func();
            var time = 1;
            while (!result && time++ < maxRetryTimes)
            {
                result = await func();
            }
            return result;
        }

        public static bool TryInvoke(Func<bool> func, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func();
            var time = 1;
            while (!result && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func();
            }
            return result;
        }

        public static async Task<bool> TryInvokeAsync(Func<Task<bool>> func, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func();
            var time = 1;
            while (!result && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func();
            }
            return result;
        }

        #region TryInvoke

        public static TResult TryInvoke<TResult>(Func<TResult> func, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func();
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = func();
            }
            return result;
        }

        public static TResult TryInvoke<T1, TResult>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func(t1);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = func(t1);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func(t1, t2);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = func(t1, t2);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func(t1, t2, t3);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = func(t1, t2, t3);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func(t1, t2, t3, t4);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = func(t1, t2, t3, t4);
            }
            return result;
        }

        #endregion TryInvoke

        #region TryInvokeWithDelay

        public static TResult TryInvoke<TResult>(Func<TResult> func, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func();
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func();
            }
            return result;
        }

        public static TResult TryInvoke<T1, TResult>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func(t1);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func(t1);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func(t1, t2);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func(t1, t2);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func(t1, t2, t3);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func(t1, t2, t3);
            }
            return result;
        }

        public static TResult TryInvoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = func(t1, t2, t3, t4);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++))));
                result = func(t1, t2, t3, t4);
            }
            return result;
        }

        #endregion TryInvokeWithDelay

        #region TryInvokeAsync

        public static async Task<TResult> TryInvokeAsync<TResult>(Func<Task<TResult>> func, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func();
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = await func();
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, TResult>(Func<T1, Task<TResult>> func, T1 t1, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func(t1);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = await func(t1);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, TResult>(Func<T1, T2, Task<TResult>> func, T1 t1, T2 t2, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = await func(t1, t2);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult>> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2, t3);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = await func(t1, t2, t3);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2, t3, t4);
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                result = await func(t1, t2, t3, t4);
            }
            return result;
        }

        #endregion TryInvokeAsync

        #region TryInvokeWithDelayAsync

        public static async Task<TResult> TryInvokeAsync<TResult>(Func<Task<TResult>> func, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func();
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func();
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, TResult>(Func<T1, Task<TResult>> func, T1 t1, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func(t1);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func(t1);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, TResult>(Func<T1, T2, Task<TResult>> func, T1 t1, T2 t2, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func(t1, t2);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult>> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2, t3);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func(t1, t2, t3);
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> validFunc, TimeSpan delay, int maxRetryTimes = 3)
        {
            var result = await func(t1, t2, t3, t4);
            var time = 1;
            while (!validFunc(result) && time < maxRetryTimes)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(delay.TotalSeconds, time++)));
                result = await func(t1, t2, t3, t4);
            }
            return result;
        }

        #endregion TryInvokeWithDelayAsync
    }
}
