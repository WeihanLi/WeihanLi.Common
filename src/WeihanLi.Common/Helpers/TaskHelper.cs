using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    public static class TaskHelper
    {
        public static Task<T> FromDefault<T>() => Task.FromResult(default(T));

        public static Func<Task> FromAction(Action action)
        {
            return () =>
            {
                action.Invoke();
#if NET45
                return Task.FromResult(1);
#else
                return Task.CompletedTask;
#endif
            };
        }

        public static Func<T, Task> FromAction<T>(Action<T> action)
        {
            return (t) =>
            {
                action.Invoke(t);
#if NET45
                return Task.FromResult(1);
#else
                return Task.CompletedTask;
#endif
            };
        }

        public static Func<T1, T2, Task> FromAction<T1, T2>(Action<T1, T2> action)
        {
            return (t1, t2) =>
            {
                action.Invoke(t1, t2);
#if NET45
                return Task.FromResult(1);
#else
                return Task.CompletedTask;
#endif
            };
        }

        public static Func<T1, T2, T3, Task> FromAction<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return (t1, t2, t3) =>
            {
                action.Invoke(t1, t2, t3);
#if NET45
                return Task.FromResult(1);
#else
                return Task.CompletedTask;
#endif
            };
        }

        public static Func<T1, T2, T3, T4, Task> FromAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            return (t1, t2, t3, t4) =>
            {
                action.Invoke(t1, t2, t3, t4);
#if NET45
                return Task.FromResult(1);
#else
                return Task.CompletedTask;
#endif
            };
        }
    }
}
