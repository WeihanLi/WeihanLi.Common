// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public sealed class BoundedConcurrentQueueTest
{
    [Fact]
    public void FullQueue_DropWrite()
    {
        var queue = new BoundedConcurrentQueue<object?>(1);
        Assert.True(queue.TryEnqueue(null));
        Assert.Equal(1, queue.Count);
        Assert.False(queue.TryEnqueue(null));
        Assert.Equal(1, queue.Count);
    }
    
    [Fact]
    public void FullQueue_DropOldest()
    {
        var queue = new BoundedConcurrentQueue<object?>(1, BoundedQueueFullMode.DropOldest);
        Assert.True(queue.TryEnqueue(null));
        Assert.Equal(1, queue.Count);
        Assert.True(queue.TryEnqueue(null));
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void NonBounded()
    {
        var queue = new BoundedConcurrentQueue<object?>();
        Assert.True(queue.TryEnqueue(null));
        Assert.Equal(1, queue.Count);
        Assert.True(queue.TryEnqueue(null));
        Assert.Equal(2, queue.Count);
    }
}
