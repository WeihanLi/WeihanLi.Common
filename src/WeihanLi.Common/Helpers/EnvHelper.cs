// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Helpers;

public static class EnvHelper
{
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string envName, string? defaultValue = null)
    {
        return Environment.GetEnvironmentVariable(envName) ?? defaultValue;
    }
}
