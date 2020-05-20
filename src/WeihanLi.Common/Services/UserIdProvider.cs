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

    public class EnvironmentUserIdProvider : IUserIdProvider
    {
        public EnvironmentUserIdProvider()
        {
        }

        public static Lazy<EnvironmentUserIdProvider> Instance = new Lazy<EnvironmentUserIdProvider>(() => new EnvironmentUserIdProvider());

        public virtual string GetUserId() => Environment.UserName;
    }

    public class ThreadPrincipalUserIdProvider : IUserIdProvider
    {
        public static Lazy<ThreadPrincipalUserIdProvider> Instance = new Lazy<ThreadPrincipalUserIdProvider>(() => new ThreadPrincipalUserIdProvider());

        public ThreadPrincipalUserIdProvider()
        {
        }

        public virtual string GetUserId() => Thread.CurrentPrincipal?.Identity?.Name;
    }
}
