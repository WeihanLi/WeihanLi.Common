// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Services;

public interface IWrapper<out T>
{
    T Value { get; }
}

public class Wrapper<T>(T value) : IWrapper<T>
{
    public T Value { get; } = value;
}
