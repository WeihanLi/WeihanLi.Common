using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers
{
    public interface IPipelineBuilder<TContext>
    {
        IPipelineBuilder<TContext> Use(Func<Action<TContext>, Action<TContext>> middleware);

        Action<TContext> Build();
    }

    public class PipelineBuilder
    {
        public static IPipelineBuilder<TContext> Create<TContext>(Action<TContext> completeAction)
        {
            return PipelineBuilder<TContext>.New(completeAction);
        }

        public static IAsyncPipelineBuilder<TContext> CreateAsync<TContext>(Func<TContext, Task> completeFunc)
        {
            return AsyncPipelineBuilder<TContext>.New(completeFunc);
        }
    }

    internal class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
    {
        private readonly Action<TContext> _completeFunc;
        private readonly IList<Func<Action<TContext>, Action<TContext>>> _pipelines = new List<Func<Action<TContext>, Action<TContext>>>();

        public PipelineBuilder(Action<TContext> completeFunc)
        {
            _completeFunc = completeFunc;
        }

        public IPipelineBuilder<TContext> Use(Func<Action<TContext>, Action<TContext>> middleware)
        {
            _pipelines.Add(middleware);
            return this;
        }

        public static PipelineBuilder<TContext> New(Action<TContext> completeFunc)
        {
            return new PipelineBuilder<TContext>(completeFunc);
        }

        public Action<TContext> Build()
        {
            var request = _completeFunc;
            foreach (var pipeline in _pipelines.Reverse())
            {
                request = pipeline(request);
            }
            return request;
        }
    }
}
