// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public static class TaskHelper
{   
    public static ValueTask ToTask(object? obj)
    {
        var task = obj switch 
        {
            ValueTask vt => vt,
            Task t => new ValueTask(t),
            _ => 
#if NET6_0_OR_GREATER
        ValueTask.CompletedTask
#else
    default
#endif
        };
        return task;
    }
}
