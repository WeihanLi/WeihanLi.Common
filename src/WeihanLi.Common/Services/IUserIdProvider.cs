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
            return userIdProvider.GetUserId().To<T>();
        }

        public static T GetUserId<T>(this IUserIdProvider userIdProvider, T defaultValue)
        {
            return userIdProvider.GetUserId().ToOrDefault(defaultValue);
        }
    }
}
