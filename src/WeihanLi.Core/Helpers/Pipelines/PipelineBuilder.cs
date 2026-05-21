// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.


// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public interface IPipelineBuilder<TContext>
{
    IPipelineBuilder<TContext> Use(Func<Action<TContext>, Action<TContext>> middleware);

    Action<TContext> Build();

    IPipelineBuilder<TContext> New();
}

internal sealed class PipelineBuilder<TContext>(Action<TContext> completeFunc) : IPipelineBuilder<TContext>
{
    private readonly Action<TContext> _completeFunc = completeFunc;
    private readonly List<Func<Action<TContext>, Action<TContext>>> _pipelines = [];

    public IPipelineBuilder<TContext> Use(Func<Action<TContext>, Action<TContext>> middleware)
    {
        _pipelines.Add(middleware);
        return this;
    }

    public Action<TContext> Build()
    {
        var request = _completeFunc;

        for (var i = _pipelines.Count - 1; i >= 0; i--)
        {
            request = _pipelines[i](request);
        }

        return request;
    }

    public IPipelineBuilder<TContext> New() => new PipelineBuilder<TContext>(_completeFunc);
}

