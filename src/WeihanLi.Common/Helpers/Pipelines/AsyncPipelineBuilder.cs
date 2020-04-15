using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers
{
    public interface IAsyncPipelineBuilder<TContext>
    {
        IAsyncPipelineBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware);

        Func<TContext, Task> Build();
    }

    internal class AsyncPipelineBuilder<TContext> : IAsyncPipelineBuilder<TContext>
    {
        private readonly Func<TContext, Task> _completeFunc;
        private readonly List<Func<Func<TContext, Task>, Func<TContext, Task>>> _pipelines = new List<Func<Func<TContext, Task>, Func<TContext, Task>>>();

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
                var pipeline = _pipelines[i];
                request = pipeline(request);
            }
            return request;
        }
    }
}
