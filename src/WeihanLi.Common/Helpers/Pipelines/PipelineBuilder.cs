using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers
{
    public interface IPipelineBuilder<TContext>
    {
        IPipelineBuilder<TContext> Use(Func<Action<TContext>, Action<TContext>> middleware);

        Action<TContext> Build();

        IPipelineBuilder<TContext> New();
    }

    internal class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
    {
        private readonly Action<TContext> _completeFunc;
        private readonly List<Func<Action<TContext>, Action<TContext>>> _pipelines = new List<Func<Action<TContext>, Action<TContext>>>();

        public PipelineBuilder(Action<TContext> completeFunc)
        {
            _completeFunc = completeFunc;
        }

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
}
