// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Services;

/// <summary>
/// Provides access to a wrapped value.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
public interface IWrapper<out T>
{
    /// <summary>
    /// Gets the wrapped value.
    /// </summary>
    T Value { get; }
}

/// <summary>
/// Default implementation of <see cref="IWrapper{T}"/>.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
/// <param name="value">The value to wrap.</param>
public class Wrapper<T>(T value) : IWrapper<T>
{
    /// <summary>
    /// Gets the wrapped value.
    /// </summary>
    public T Value { get; } = value;
}
