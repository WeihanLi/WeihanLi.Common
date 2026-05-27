// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Specifies a data operation type.
/// </summary>
public enum DataOperationType
{
    /// <summary>
    /// Query operation.
    /// </summary>
    Query = 0,

    /// <summary>
    /// Add operation.
    /// </summary>
    Add = 1,

    /// <summary>
    /// Delete operation.
    /// </summary>
    Delete = 2,

    /// <summary>
    /// Update operation.
    /// </summary>
    Update = 3,
}
