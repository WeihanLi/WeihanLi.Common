// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class TenantInfo<TKey>
{
    public TKey? TenantId { get; set; }

    public string? TenantName { get; set; }
}

public class TenantInfo : TenantInfo<string>;
