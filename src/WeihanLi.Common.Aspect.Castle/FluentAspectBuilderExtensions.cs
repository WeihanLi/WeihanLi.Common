using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect.Castle;

public static class FluentAspectBuilderExtensions
{
    public static IFluentAspectsBuilder UseCastleProxy(this IFluentAspectsBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddTransient<IProxyFactory, CastleProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, CastleProxyTypeFactory>();

        FluentAspects.AspectOptions.ProxyFactory = CastleProxyFactory.Instance;

        return builder;
    }

    public static IFluentAspectsServiceContainerBuilder UseCastleProxy(this IFluentAspectsServiceContainerBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddTransient<IProxyFactory, CastleProxyFactory>();
        builder.Services.AddTransient<IProxyTypeFactory, CastleProxyTypeFactory>();

        FluentAspects.AspectOptions.ProxyFactory = CastleProxyFactory.Instance;

        return builder;
    }
}
