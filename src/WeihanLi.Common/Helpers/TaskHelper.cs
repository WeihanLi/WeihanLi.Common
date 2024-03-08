// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public static class TaskHelper
{
    public static Task ToTask(object? obj)
    {
        var task = obj switch
        {
            ValueTask vt => vt.AsTask(),
            Task t => t,
            _ => Task.CompletedTask
        };
        return task;
    }
}
