using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect.AspectCore
{
    public static class FluentAspectBuilderExtensions
    {
        public static IFluentAspectBuilder UseAspectCoreProxy(this IFluentAspectBuilder builder)
        {
            builder.Services.AddTransient<IProxyFactory, AspectCoreProxyFactory>();
            FluentAspects.AspectOptions.ProxyFactory = new AspectCoreProxyFactory(DependencyResolver.Current);

            return builder;
        }
    }
}
