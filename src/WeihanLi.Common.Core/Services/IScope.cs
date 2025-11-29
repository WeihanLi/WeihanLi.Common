// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Services;

public interface IScope : IDisposable;

public sealed class NullScope : IScope
{
    public void Dispose()
    {
    }

    public static NullScope Instance { get; } = new();
}
