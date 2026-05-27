using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

/// <summary>
/// Provides current tenant information.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the current tenant identifier.
    /// </summary>
    /// <returns>The current tenant identifier, or <see langword="null"/> when no tenant is available.</returns>
    string? GetTenantId();

    /// <summary>
    /// Gets the current tenant information.
    /// </summary>
    /// <returns>The current tenant information, or <see langword="null"/> when no tenant is available.</returns>
    TenantInfo? GetTenantInfo();
}

/// <summary>
/// Extension methods for <see cref="ITenantProvider"/>.
/// </summary>
public static class TenantIdProviderExtensions
{
#if NET
    /// <summary>
    /// Gets the current tenant identifier and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The target tenant identifier type.</typeparam>
    /// <param name="tenantIdProvider">The tenant provider.</param>
    /// <param name="defaultValue">The value to return when no tenant identifier is available or conversion fails.</param>
    /// <returns>The converted tenant identifier, or <paramref name="defaultValue"/>.</returns>
    public static T? GetTenantId<T>(this ITenantProvider tenantIdProvider, T? defaultValue = default) 
        where T: IParsable<T>
    {
        return tenantIdProvider.GetTenantId().ToOrDefault(defaultValue: defaultValue);
    }

    /// <summary>
    /// Tries to get the current tenant identifier and convert it to the specified type.
    /// </summary>
    /// <typeparam name="T">The target tenant identifier type.</typeparam>
    /// <param name="tenantIdProvider">The tenant provider.</param>
    /// <param name="value">When this method returns, contains the converted tenant identifier or <paramref name="defaultValue"/>.</param>
    /// <param name="defaultValue">The fallback value used when no tenant identifier is available or conversion fails.</param>
    /// <returns><see langword="true"/> when the tenant identifier is available and converted successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetTenantId<T>(this ITenantProvider tenantIdProvider, out T? value, T? defaultValue = default)
        where T: IParsable<T>
    {
        try
        {
            var tenantId = tenantIdProvider.GetTenantId();
            if (!string.IsNullOrEmpty(tenantId))
            {
                value = tenantId.To<T>();
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
