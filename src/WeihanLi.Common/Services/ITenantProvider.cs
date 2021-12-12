using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

public interface ITenantProvider
{
    string GetTenantId();

    TenantInfo GetTenantInfo();
}

public static class TenantIdProviderExtensions
{
    public static T? GetTenantId<T>(this ITenantProvider tenantIdProvider, T? defaultValue = default)
    {
        return tenantIdProvider.GetTenantId().ToOrDefault(defaultValue);
    }

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
