using System.Diagnostics.CodeAnalysis;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

public interface IUserIdProvider
{
    string? GetUserId();
}

public static class UserIdProviderExtensions
{
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? GetUserId<T>(this IUserIdProvider userIdProvider, T? defaultValue = default)
    {
        return userIdProvider.GetUserId().ToOrDefault(defaultValue);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static bool TryGetUserId<T>(this IUserIdProvider userIdProvider, out T? value, T? defaultValue = default)
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
    public static readonly Lazy<EnvironmentUserIdProvider> Instance = new(() => new EnvironmentUserIdProvider());

    public virtual string GetUserId() => Environment.UserName;
}
