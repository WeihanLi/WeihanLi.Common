// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public static class PipelineBuilderExtensions
{
    #region IPipelineBuilder

    public static IPipelineBuilder<TContext> Use<TContext>(this IPipelineBuilder<TContext> builder, Action<TContext, Action> action)

    {
        return builder.Use(next =>
            context =>
            {
                action(context, () => next(context));
            });
    }

    public static IPipelineBuilder<TContext> Use<TContext>(this IPipelineBuilder<TContext> builder, Action<TContext, Action<TContext>> action)
    {
        return builder.Use(next =>
            context =>
            {
                action(context, next);
            });
    }

    public static IPipelineBuilder<TContext> Run<TContext>(this IPipelineBuilder<TContext> builder, Action<TContext> handler)
    {
        return builder.Use(_ => handler);
    }

    public static IPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IPipelineBuilder<TContext> builder, TMiddleware middleware)
        where TMiddleware : class, IPipelineMiddleware<TContext>
    {
        Guard.NotNull(middleware);
        return builder.Use(next =>
            context =>
            {
                middleware.Invoke(context, next);
            });
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IPipelineBuilder<TContext> builder)
      where TMiddleware : class, IPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
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

    public static IPipelineBuilder<TContext> UseWhen<TContext>(this IPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IPipelineBuilder<TContext>> configureAction)
    {
        var branchPipelineBuilder = builder.New();
        configureAction(branchPipelineBuilder);
        builder.Use((context, next) =>
        {
            branchPipelineBuilder.Run(_ => next(context));
            var branch = branchPipelineBuilder.Build();
            if (predict.Invoke(context))
            {
                branch(context);
            }
            else
            {
                next(context);
            }
        });

        return builder;
    }

    #endregion IPipelineBuilder

    #region IAsyncPipelineBuilder

    public static IAsyncPipelineBuilder<TContext> Use<TContext>(this IAsyncPipelineBuilder<TContext> builder,
        Func<TContext, Func<Task>, Task> func)
    {
        return builder.Use(next =>
            context =>
            {
                return func(context, () => next(context));
            });
    }

    public static IAsyncPipelineBuilder<TContext> Use<TContext>(this IAsyncPipelineBuilder<TContext> builder,
        Func<TContext, Func<TContext, Task>, Task> func)
    {
        return builder.Use(next =>
            context => func(context, next));
    }

    public static IAsyncPipelineBuilder<TContext> UseMiddleware<TContext>(this IAsyncPipelineBuilder<TContext> builder, IAsyncPipelineMiddleware<TContext> middleware)
    {
        Guard.NotNull(middleware);
        return builder.Use(next =>
            async context =>
            {
                await middleware.InvokeAsync(context, next);
            });
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IAsyncPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IAsyncPipelineBuilder<TContext> builder)
        where TMiddleware : class, IAsyncPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
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

    public static IAsyncPipelineBuilder<TContext> UseWhen<TContext>(this IAsyncPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IAsyncPipelineBuilder<TContext>> configureAction)
    {
        var branchPipelineBuilder = builder.New();
        configureAction(branchPipelineBuilder);
        builder.Use((context, next) =>
        {
            branchPipelineBuilder.Run(_ => next(context));
            var branch = branchPipelineBuilder.Build();
            if (predict.Invoke(context))
            {
                return branch(context);
            }
            return next(context);
        });

        return builder;
    }

    public static IAsyncPipelineBuilder<TContext> Run<TContext>(this IAsyncPipelineBuilder<TContext> builder, Func<TContext, Task> handler)
    {
        return builder.Use(_ => handler);
    }
    #endregion IAsyncPipelineBuilder

    #region IValueAsyncPipelineBuilder

    public static IValueAsyncPipelineBuilder<TContext> Use<TContext>(this IValueAsyncPipelineBuilder<TContext> builder,
        Func<TContext, Func<ValueTask>, ValueTask> func)
    {
        return builder.Use(next =>
            context =>
            {
                return func(context, () => next(context));
            });
    }

    public static IValueAsyncPipelineBuilder<TContext> Use<TContext>(this IValueAsyncPipelineBuilder<TContext> builder,
        Func<TContext, Func<TContext, ValueTask>, ValueTask> func)
    {
        return builder.Use(next =>
            context => func(context, next));
    }

    public static IValueAsyncPipelineBuilder<TContext> UseMiddleware<TContext>(this IValueAsyncPipelineBuilder<TContext> builder, IValueAsyncPipelineMiddleware<TContext> middleware)
    {
        Guard.NotNull(middleware);
        return builder.Use(next =>
            async context =>
            {
                await middleware.InvokeAsync(context, next);
                await next(context);
            });
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IValueAsyncPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IValueAsyncPipelineBuilder<TContext> builder)
        where TMiddleware : class, IValueAsyncPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
    }

    public static IValueAsyncPipelineBuilder<TContext> When<TContext>(this IValueAsyncPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IValueAsyncPipelineBuilder<TContext>> configureAction)
    {
        builder.Use((context, next) =>
        {
            var branchPipelineBuilder = builder.New();
            configureAction(branchPipelineBuilder);
            if (predict.Invoke(context))
            {
                var branchPipeline = branchPipelineBuilder.Build();
                return branchPipeline.Invoke(context);
            }

            return next(context);
        });

        return builder;
    }

    public static IValueAsyncPipelineBuilder<TContext> UseWhen<TContext>(this IValueAsyncPipelineBuilder<TContext> builder, Func<TContext, bool> predict, Action<IValueAsyncPipelineBuilder<TContext>> configureAction)
    {
        var branchPipelineBuilder = builder.New();
        configureAction(branchPipelineBuilder);
        builder.Use((context, next) =>
        {
            branchPipelineBuilder.Run(_ => next(context));
            var branch = branchPipelineBuilder.Build();
            if (predict.Invoke(context))
            {
                return branch(context);
            }
            return next(context);
        });

        return builder;
    }

    public static IValueAsyncPipelineBuilder<TContext> Run<TContext>(this IValueAsyncPipelineBuilder<TContext> builder, Func<TContext, ValueTask> handler)
    {
        return builder.Use(_ => handler);
    }

    #endregion IValueAsyncPipelineBuilder
}
