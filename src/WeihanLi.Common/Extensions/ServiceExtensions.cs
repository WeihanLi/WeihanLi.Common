// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Services;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ServiceExtensions
{
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? GetTenantId<T>(this ITenantProvider tenantIdProvider, T? defaultValue = default)
    {
        return tenantIdProvider.GetTenantId().ToOrDefault(defaultValue: defaultValue);
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
