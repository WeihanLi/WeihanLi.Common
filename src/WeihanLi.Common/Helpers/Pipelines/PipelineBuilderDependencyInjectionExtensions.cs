// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Helpers;

public static class PipelineBuilderDependencyInjectionExtensions
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IPipelineBuilder<TContext> builder)
        where TMiddleware : class, IPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IAsyncPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IAsyncPipelineBuilder<TContext> builder)
        where TMiddleware : class, IAsyncPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IValueAsyncPipelineBuilder<TContext> UseMiddleware<TContext, TMiddleware>(this IValueAsyncPipelineBuilder<TContext> builder)
        where TMiddleware : class, IValueAsyncPipelineMiddleware<TContext>
    {
        return builder.UseMiddleware(DependencyResolver.Current.GetServiceOrCreateInstance<TMiddleware>());
    }
}
