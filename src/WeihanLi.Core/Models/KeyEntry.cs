// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a key entry that maps a property to a column and value.
/// </summary>
public class KeyEntry
{
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the column name.
    /// </summary>
    public string ColumnName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the key value.
    /// </summary>
    public object? Value { get; set; }
}
