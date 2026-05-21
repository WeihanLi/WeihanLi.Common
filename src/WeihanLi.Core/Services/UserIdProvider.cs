using System.Diagnostics.CodeAnalysis;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

public interface IUserIdProvider
{
    string? GetUserId();
}

public static class UserIdProviderExtensions
{
#if NET
    public static T? GetUserId<T>(this IUserIdProvider userIdProvider, T? defaultValue = default)
        where T: ISpanParsable<T>
    {
        var userId = userIdProvider.GetUserId();
        return string.IsNullOrEmpty(userId)
            ? defaultValue
            : userId.AsSpan().ToOrDefault(defaultValue: defaultValue);
    }

    public static bool TryGetUserId<T>(this IUserIdProvider userIdProvider, out T? value, T? defaultValue = default)
        where T: ISpanParsable<T>
    {
        try
        {
            var userId = userIdProvider.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                value = userId.AsSpan().To<T>();
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
#endif
}

public class EnvironmentUserIdProvider : IUserIdProvider
{
    public static EnvironmentUserIdProvider Instance { get; } = new();

    public virtual string GetUserId() => Environment.UserName;
}

public sealed class DelegateUserIdProvider(Func<string?> userIdFactory) : IUserIdProvider
{
    public DelegateUserIdProvider(string userId) : this(() => userId)
    {
    }

    public string? GetUserId() => userIdFactory();
}
