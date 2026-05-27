// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents tenant information.
/// </summary>
/// <typeparam name="TKey">The tenant identifier type.</typeparam>
public class TenantInfo<TKey>
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public TKey? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the tenant name.
    /// </summary>
    public string? TenantName { get; set; }
}

/// <summary>
/// Represents tenant information with a string tenant identifier.
/// </summary>
public class TenantInfo : TenantInfo<string>;
