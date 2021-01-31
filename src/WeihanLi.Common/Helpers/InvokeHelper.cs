using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WeihanLi.Common.Logging;

namespace WeihanLi.Common.Helpers
{
    public static class InvokeHelper
    {
        static InvokeHelper()
        {
            OnInvokeException = ex => LogHelper.GetLogger(typeof(InvokeHelper)).Error(ex);
        }

        #region Profile

        public static long Profile(Action action)
        {
            Guard.NotNull(action, nameof(action));
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static long Profile<T>(Action<T> action, T t)
        {
            Guard.NotNull(action, nameof(action));
            var stopwatch = Stopwatch.StartNew();
            action(t);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static long Profile<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Guard.NotNull(action, nameof(action));
            var stopwatch = Stopwatch.StartNew();
            action(t1, t2);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static long Profile<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            var stopwatch = Stopwatch.StartNew();
            action(t1, t2, t3);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static async Task<long> ProfileAsync(Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();
            await action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static async Task<long> ProfileAsync<T>(Func<T, Task> func, T t)
        {
            var stopwatch = Stopwatch.StartNew();
            await func(t);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static async Task<long> ProfileAsync<T1, T2>(Func<T1, T2, Task> func, T1 t1, T2 t2)
        {
            var stopwatch = Stopwatch.StartNew();
            await func(t1, t2);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public static async Task<long> ProfileAsync<T1, T2, T3>(Func<T1, T2, T3, Task> func, T1 t1, T2 t2, T3 t3)
        {
            var stopwatch = Stopwatch.StartNew();
            await func(t1, t2, t3);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        #endregion Profile

        #region TryInvoke

        public static Action<Exception>? OnInvokeException { get; set; }

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
}
