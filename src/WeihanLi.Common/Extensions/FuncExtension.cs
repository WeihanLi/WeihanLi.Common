using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class FuncExtension
    {
        public static Func<Task> WrapTask(this Action action)
        {
            return () =>
            {
                action.Invoke();
                return Task.CompletedTask;
            };
        }

        public static Func<T, Task> WrapTask<T>(this Action<T> action)
        {
            return (t) =>
            {
                action.Invoke(t);
                return Task.CompletedTask;
            };
        }

        public static Func<T1, T2, Task> WrapTask<T1, T2>(this Action<T1, T2> action)
        {
            return (t1, t2) =>
            {
                action.Invoke(t1, t2);
                return Task.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, Task> WrapTask<T1, T2, T3>(this Action<T1, T2, T3> action)
        {
            return (t1, t2, t3) =>
            {
                action.Invoke(t1, t2, t3);
                return Task.CompletedTask;
            };
        }

        public static Func<T1, T2, T3, T4, Task> WrapTask<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        {
            return (t1, t2, t3, t4) =>
            {
                action.Invoke(t1, t2, t3, t4);
                return Task.CompletedTask;
            };
        }

        public static Func<CancellationToken, Task> WrapCancellation(this Func<Task> func) => cancellationToken => func?.Invoke();

        public static Func<T, CancellationToken, Task> WrapCancellation<T>(this Func<T, Task> func) => (t, cancellationToken) => func?.Invoke(t);

        public static Func<T1, T2, CancellationToken, Task> WrapCancellation<T1, T2>(this Func<T1, T2, Task> func) => (t1, t2, cancellationToken) => func?.Invoke(t1, t2);

        public static Func<T1, T2, T3, CancellationToken, Task> WrapCancellation<T1, T2, T3>(this Func<T1, T2, T3, Task> func) => (t1, t2, t3, cancellationToken) => func?.Invoke(t1, t2, t3);

        public static Func<T1, T2, T3, T4, CancellationToken, Task> WrapCancellation<T1, T2, T3, T4>(this Func<T1, T2, T3, T4, Task> func) => (t1, t2, t3, t4, cancellationToken) => func?.Invoke(t1, t2, t3, t4);
    }
}
