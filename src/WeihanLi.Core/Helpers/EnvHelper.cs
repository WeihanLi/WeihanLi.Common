// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class EnvHelper
{
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string envName, string? defaultValue = null)
    {
        return Environment.GetEnvironmentVariable(envName) ?? defaultValue;
    }

    public static bool BooleanVal(string envName, bool defaultValue = false)
    {
        var val = Environment.GetEnvironmentVariable(envName);
        return val.ToBoolean(defaultValue);
    }
}
