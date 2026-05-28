using System.Diagnostics.CodeAnalysis;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

/// <summary>
/// Provides the current user identifier.
/// </summary>
public interface IUserIdProvider
{
    /// <summary>
    /// Gets the current user identifier.
    /// </summary>
    /// <returns>The current user identifier, or <see langword="null"/> when no user is available.</returns>
    string? GetUserId();
}

/// <summary>
/// Extension methods for <see cref="IUserIdProvider"/>.
/// </summary>
public static class UserIdProviderExtensions
{
#if NET
    /// <summary>
    /// Gets the current user identifier and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The target user identifier type.</typeparam>
    /// <param name="userIdProvider">The user identifier provider.</param>
    /// <param name="defaultValue">The value to return when no user identifier is available or conversion fails.</param>
    /// <returns>The converted user identifier, or <paramref name="defaultValue"/>.</returns>
    public static T? GetUserId<T>(this IUserIdProvider userIdProvider, T? defaultValue = default)
        where T: ISpanParsable<T>
    {
        var userId = userIdProvider.GetUserId();
        return string.IsNullOrEmpty(userId)
            ? defaultValue
            : userId.AsSpan().ToOrDefault(defaultValue: defaultValue);
    }

    /// <summary>
    /// Tries to get the current user identifier and convert it to the specified type.
    /// </summary>
    /// <typeparam name="T">The target user identifier type.</typeparam>
    /// <param name="userIdProvider">The user identifier provider.</param>
    /// <param name="value">When this method returns, contains the converted user identifier or <paramref name="defaultValue"/>.</param>
    /// <param name="defaultValue">The fallback value used when no user identifier is available or conversion fails.</param>
    /// <returns><see langword="true"/> when the user identifier is available and converted successfully; otherwise, <see langword="false"/>.</returns>
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

/// <summary>
/// User identifier provider that returns <see cref="Environment.UserName"/>.
/// </summary>
public class EnvironmentUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static EnvironmentUserIdProvider Instance { get; } = new();

    /// <inheritdoc />
    public virtual string GetUserId() => Environment.UserName;
}

/// <summary>
/// User identifier provider backed by a delegate.
/// </summary>
/// <param name="userIdFactory">The delegate used to resolve the current user identifier.</param>
public sealed class DelegateUserIdProvider(Func<string?> userIdFactory) : IUserIdProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateUserIdProvider"/> class with a fixed user identifier.
    /// </summary>
    /// <param name="userId">The fixed user identifier.</param>
    public DelegateUserIdProvider(string userId) : this(() => userId)
    {
    }

    /// <inheritdoc />
    public string? GetUserId() => userIdFactory();
}
