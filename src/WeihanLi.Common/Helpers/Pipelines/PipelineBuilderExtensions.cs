using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers
{
    public static class PipelineBuilderExtensions
    {
        public static IPipelineBuilder<TContext> Use<TContext>(this IPipelineBuilder<TContext> builder, Action<TContext, Action> action)

        {
            return builder.Use(next =>
                context =>
                {
                    action(context, () => next(context));
                });
        }

        public static IPipelineBuilder<TContext> Run<TContext>(this IPipelineBuilder<TContext> builder, Action<TContext> handler)
        {
            return builder.Use(_ => handler);
        }

        public static IPipelineBuilder<TContext> When<TContext>(this IPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IPipelineBuilder<TContext>> configureAction)
        {
            builder.Use((context, next) =>
            {
                if (predict.Invoke(context))
                {
                    var branchPipelineBuilder = builder.New();
                    configureAction(branchPipelineBuilder);
                    var branchPipeline = branchPipelineBuilder.Build();
                    branchPipeline.Invoke(context);
                }
                else
                {
                    next();
                }
            });

            return builder;
        }

        public static IAsyncPipelineBuilder<TContext> When<TContext>(this IAsyncPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IAsyncPipelineBuilder<TContext>> configureAction)
        {
            builder.Use((context, next) =>
            {
                if (predict.Invoke(context))
                {
                    var branchPipelineBuilder = builder.New();
                    configureAction(branchPipelineBuilder);
                    var branchPipeline = branchPipelineBuilder.Build();
                    return branchPipeline.Invoke(context);
                }

                return next();
            });

            return builder;
        }

        public static IAsyncPipelineBuilder<TContext> Use<TContext>(this IAsyncPipelineBuilder<TContext> builder,
            Func<TContext, Func<Task>, Task> func)
        {
            return builder.Use(next =>
                context =>
                {
                    return func(context, () => next(context));
                });
        }

        public static IAsyncPipelineBuilder<TContext> Run<TContext>(this IAsyncPipelineBuilder<TContext> builder, Func<TContext, Task> handler)
        {
            return builder.Use(_ => handler);
        }
    }
}
