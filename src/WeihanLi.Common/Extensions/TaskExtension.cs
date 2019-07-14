using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class TaskExtension
    {
        public static Task WhenAny(this IEnumerable<Task> tasks) => Task.WhenAny(tasks);

        public static Task<Task<TResult>> WhenAny<TResult>(this IEnumerable<Task<TResult>> tasks)
            => Task.WhenAny(tasks);

        public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

        public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> tasks)
            => Task.WhenAll(tasks);
    }
}
