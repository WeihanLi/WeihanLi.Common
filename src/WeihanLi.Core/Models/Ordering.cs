// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Specifies sort ordering.
/// </summary>
public enum Ordering
{
    /// <summary>
    /// Ascending order.
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Descending order.
    /// </summary>
    Descending = 1
}

/// <summary>
/// Specifies which bounds are included in a range.
/// </summary>
[Flags]
public enum RangeInclusion
{
    /// <summary>
    /// No bounds are included.
    /// </summary>
    None = 0,

    /// <summary>
    /// The lower bound is included.
    /// </summary>
    IncludeLowerBound = 1,

    /// <summary>
    /// The upper bound is included.
    /// </summary>
    IncludeUpperBound = 2
}
