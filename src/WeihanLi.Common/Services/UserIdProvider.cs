using System;
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

        public static readonly Lazy<EnvironmentUserIdProvider> Instance = new Lazy<EnvironmentUserIdProvider>(() => new EnvironmentUserIdProvider());

        public virtual string GetUserId() => Environment.UserName;
    }
}
