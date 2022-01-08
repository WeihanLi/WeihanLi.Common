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
}
