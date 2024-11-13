using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

public interface ITenantProvider
{
    string? GetTenantId();

    TenantInfo? GetTenantInfo();
}

public static class TenantIdProviderExtensions
{
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? GetTenantId<T>(this ITenantProvider tenantIdProvider, T? defaultValue = default)
    {
        return tenantIdProvider.GetTenantId().ToOrDefault(defaultValue);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static bool TryGetTenantId<T>(this ITenantProvider tenantIdProvider, out T? value, T? defaultValue = default)
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
}
