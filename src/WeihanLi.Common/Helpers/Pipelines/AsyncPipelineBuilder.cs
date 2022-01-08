// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public interface IAsyncPipelineBuilder<TContext>
{
    IAsyncPipelineBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware);

    Func<TContext, Task> Build();

    IAsyncPipelineBuilder<TContext> New();
}

internal sealed class AsyncPipelineBuilder<TContext> : IAsyncPipelineBuilder<TContext>
{
    private readonly Func<TContext, Task> _completeFunc;
    private readonly List<Func<Func<TContext, Task>, Func<TContext, Task>>> _pipelines = new();

    public AsyncPipelineBuilder(Func<TContext, Task> completeFunc)
    {
        _completeFunc = completeFunc;
    }

    public IAsyncPipelineBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware)
    {
        _pipelines.Add(middleware);
        return this;
    }

    public Func<TContext, Task> Build()
    {
        var request = _completeFunc;
        for (var i = _pipelines.Count - 1; i >= 0; i--)
        {
            request = _pipelines[i](request);
        }
        return request;
    }

    public IAsyncPipelineBuilder<TContext> New() => new AsyncPipelineBuilder<TContext>(_completeFunc);
}
