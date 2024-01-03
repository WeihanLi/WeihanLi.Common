// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public static class PipelineBuilder
{
    public static IPipelineBuilder<TContext> Create<TContext>()
    {
        return new PipelineBuilder<TContext>(c => { });
    }

    public static IPipelineBuilder<TContext> Create<TContext>(Action<TContext> completeAction)
    {
        return new PipelineBuilder<TContext>(completeAction);
    }

    public static IAsyncPipelineBuilder<TContext> CreateAsync<TContext>()
    {
        return new AsyncPipelineBuilder<TContext>(c => Task.CompletedTask);
    }

    public static IAsyncPipelineBuilder<TContext> CreateAsync<TContext>(Func<TContext, Task> completeFunc)
    {
        return new AsyncPipelineBuilder<TContext>(completeFunc);
    }

    public static IValueAsyncPipelineBuilder<TContext> CreateValueAsync<TContext>()
    {
        return new ValueAsyncPipelineBuilder<TContext>(c =>
#if NET6_0_OR_GREATER
            ValueTask.CompletedTask
#else
            default
#endif
        );
    }
    public static IValueAsyncPipelineBuilder<TContext> CreateValueAsync<TContext>(Func<TContext, ValueTask> completeFunc)
    {
        return new ValueAsyncPipelineBuilder<TContext>(completeFunc);
    }
}
