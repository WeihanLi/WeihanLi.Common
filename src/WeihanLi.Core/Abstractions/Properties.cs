// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Abstractions;

/// <summary>
/// Exposes an object property bag.
/// </summary>
public interface IProperties
{
    /// <summary>
    /// Gets the object property bag.
    /// </summary>
    IDictionary<string, object?> Properties { get; }
}

/// <summary>
/// Exposes a string property bag.
/// </summary>
public interface IStringProperties
{
    /// <summary>
    /// Gets the string property bag.
    /// </summary>
    IDictionary<string, string> Properties { get; }
}
