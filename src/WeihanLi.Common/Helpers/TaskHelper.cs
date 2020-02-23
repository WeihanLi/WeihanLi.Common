using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    public static class TaskHelper
    {
        public static Task<T> FromDefault<T>() => Task.FromResult(default(T));

        public static readonly Task CompletedTask =
#if NET45

            Task.FromResult(true);
#else
        Task.CompletedTask;

#endif
    }
}
