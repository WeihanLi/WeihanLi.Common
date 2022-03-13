// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Abstractions;

namespace WeihanLi.Common.Extensions;

public static class PropertiesExtensions
{
    public static T? GetProperty<T>(this IDictionary<string, object?> properties, string key)
    {
        return Guard.NotNull(properties).TryGetValue(key, out var value) ? (T?)value : default(T);
    }

    public static void SetProperty<T>(this IDictionary<string, object?> properties, string key, T value)
    {
        Guard.NotNull(properties)[key] = value;
    }

    public static T? GetProperty<T>(this IProperties propertiesHolder, string key)
    {
        Guard.NotNull(propertiesHolder);
        return propertiesHolder.Properties.TryGetValue(key, out var value) ? (T?)value : default(T);
    }

    public static void SetProperty<T>(this IProperties propertiesHolder, string key, T value)
    {
        Guard.NotNull(propertiesHolder);
        propertiesHolder.Properties[key] = value;
    }
}
