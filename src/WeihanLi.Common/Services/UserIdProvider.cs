using System;
using System.Threading;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services
{
    public interface IUserIdProvider
    {
        string GetUserId();
    }

    public static class UserIdProviderExtensions
    {
        public static T GetUserId<T>(this IUserIdProvider userIdProvider)
        {
            return userIdProvider.GetUserId().ToOrDefault<T>();
        }

        public static bool TryGetUserId<T>(this IUserIdProvider userIdProvider, out T value, T defaultValue = default)
        {
            try
            {
                var userId = userIdProvider.GetUserId();
                if (!string.IsNullOrEmpty(userId))
                {
                    value = userId.To<T>();
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            value = defaultValue;
            return false;
        }
    }

    public sealed class EnvironmentUserIdProvider : IUserIdProvider
    {
        private EnvironmentUserIdProvider()
        {
        }

        public static Lazy<EnvironmentUserIdProvider> Instance = new Lazy<EnvironmentUserIdProvider>(() => new EnvironmentUserIdProvider());

        public string GetUserId() => Environment.UserName;
    }

    public sealed class ThreadPrincipalUserIdProvider : IUserIdProvider
    {
        public static Lazy<ThreadPrincipalUserIdProvider> Instance = new Lazy<ThreadPrincipalUserIdProvider>(() => new ThreadPrincipalUserIdProvider());

        private ThreadPrincipalUserIdProvider()
        {
        }

        public string GetUserId() => Thread.CurrentPrincipal?.Identity?.Name;
    }
}
