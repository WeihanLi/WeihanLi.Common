using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    public static class TaskHelper
    {
        public static Task<T> FromDefault<T>() => Task.FromResult(default(T));
    }
}
