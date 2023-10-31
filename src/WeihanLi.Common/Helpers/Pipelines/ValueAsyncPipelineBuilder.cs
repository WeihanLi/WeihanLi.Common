// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public interface IValueAsyncPipelineBuilder<TContext>
{
    IValueAsyncPipelineBuilder<TContext> Use(Func<Func<TContext, ValueTask>, Func<TContext, ValueTask>> middleware);

    Func<TContext, ValueTask> Build();

    IValueAsyncPipelineBuilder<TContext> New();
}

internal sealed class ValueAsyncPipelineBuilder<TContext> : IValueAsyncPipelineBuilder<TContext>
{
    private readonly Func<TContext, ValueTask> _completeFunc;
    private readonly List<Func<Func<TContext, ValueTask>, Func<TContext, ValueTask>>> _pipelines = new();

    public ValueAsyncPipelineBuilder(Func<TContext, ValueTask> completeFunc)
    {
        _completeFunc = completeFunc;
    }

    public IValueAsyncPipelineBuilder<TContext> Use(Func<Func<TContext, ValueTask>, Func<TContext, ValueTask>> middleware)
    {
        _pipelines.Add(middleware);
        return this;
    }

    public Func<TContext, ValueTask> Build()
    {
        var request = _completeFunc;
        for (var i = _pipelines.Count - 1; i >= 0; i--)
        {
            request = _pipelines[i](request);
        }
        return request;
    }

    public IValueAsyncPipelineBuilder<TContext> New() => new ValueAsyncPipelineBuilder<TContext>(_completeFunc);
}
