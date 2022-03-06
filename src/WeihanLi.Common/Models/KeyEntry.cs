// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class KeyEntry
{
    public string PropertyName { get; set; } = null!;

    public string ColumnName { get; set; } = null!;

    public object? Value { get; set; }
}
