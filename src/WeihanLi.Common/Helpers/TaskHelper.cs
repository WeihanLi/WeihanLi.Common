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

    public static Task<T> ToTask<T>(object? obj, T defaultValue = default)
    {
        var task = obj switch
        {
            ValueTask<T> vt => vt.AsTask(),
            Task<T> t => t,
            ValueTask vt0 => vt0.AsTask().ContinueWith(_ => defaultValue),
            Task t0 => t0.ContinueWith(_ => defaultValue),
            _ => Task.FromResult(defaultValue)
        };
        return task;
    }
}
