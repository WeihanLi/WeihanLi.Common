using Microsoft.Extensions.DependencyInjection;
using System;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect.AspectCore;

public static class FluentAspectBuilderExtensions
{
    public static IFluentAspectsBuilder UseAspectCoreProxy(this IFluentAspectsBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddTransient<IProxyFactory, AspectCoreProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, AspectCoreProxyTypeFactory>();
        FluentAspects.AspectOptions.ProxyFactory = AspectCoreProxyFactory.Instance;

        return builder;
    }

    public static IFluentAspectsServiceContainerBuilder UseAspectCoreProxy(this IFluentAspectsServiceContainerBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddTransient<IProxyFactory, AspectCoreProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, AspectCoreProxyTypeFactory>();
        FluentAspects.AspectOptions.ProxyFactory = AspectCoreProxyFactory.Instance;

        return builder;
    }
}
