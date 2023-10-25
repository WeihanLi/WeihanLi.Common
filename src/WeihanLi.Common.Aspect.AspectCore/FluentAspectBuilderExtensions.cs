using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect.AspectCore;

public static class FluentAspectBuilderExtensions
{
    public static IFluentAspectsBuilder UseAspectCoreProxy(this IFluentAspectsBuilder builder)
    {
        Guard.NotNull(builder);

        builder.Services.AddTransient<IProxyFactory, AspectCoreProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, AspectCoreProxyTypeFactory>();
        FluentAspects.AspectOptions.ProxyFactory = AspectCoreProxyFactory.Instance;

        return builder;
    }

    public static IFluentAspectsServiceContainerBuilder UseAspectCoreProxy(this IFluentAspectsServiceContainerBuilder builder)
    {
        Guard.NotNull(builder);

        builder.Services.AddTransient<IProxyFactory, AspectCoreProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, AspectCoreProxyTypeFactory>();
        FluentAspects.AspectOptions.ProxyFactory = AspectCoreProxyFactory.Instance;

        return builder;
    }
}
