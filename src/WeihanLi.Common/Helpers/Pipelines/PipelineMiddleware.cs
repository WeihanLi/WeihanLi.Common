// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public interface IPipelineMiddleware<TContext>
{
    void Invoke(TContext context, Action<TContext> next);
}

public interface IAsyncPipelineMiddleware<TContext>
{
    Task InvokeAsync(TContext context, Func<TContext, Task> next);
}

#if ValueTaskSupport

public interface IValueAsyncPipelineMiddleware<TContext>
{
    ValueTask InvokeAsync(TContext context, Func<TContext, ValueTask> next);
}

#endif

