using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect
{
    public interface IFluentAspectBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class FluentAspectBuilder : IFluentAspectBuilder
    {
        public FluentAspectBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }

        public IServiceCollection Services { get; }
    }

    public static class FluentAspectBuilderExtensions
    {
        public static IFluentAspectBuilder UseProxyFactory(this IFluentAspectBuilder builder, IProxyFactory proxyFactory)
        {
            builder.Services.AddSingleton(proxyFactory);
            return builder;
        }

        public static IFluentAspectBuilder UseProxyTypeFactory(this IFluentAspectBuilder builder, IProxyTypeFactory proxyTypeFactory)
        {
            builder.Services.AddSingleton(proxyTypeFactory);
            return builder;
        }

        public static IFluentAspectBuilder UseInterceptorResolver(this IFluentAspectBuilder builder, IInterceptorResolver resolver)
        {
            builder.Services.AddSingleton(resolver);
            return builder;
        }

        public static IFluentAspectBuilder UseProxyFactory<TProxyFactory>(this IFluentAspectBuilder builder) where TProxyFactory : class, IProxyFactory
        {
            builder.Services.AddSingleton<IProxyFactory, TProxyFactory>();
            return builder;
        }

        public static IFluentAspectBuilder UseProxyTypeFactory<TProxyTypeFactory>(this IFluentAspectBuilder builder) where TProxyTypeFactory : class, IProxyTypeFactory
        {
            builder.Services.AddSingleton<IProxyTypeFactory, TProxyTypeFactory>();
            return builder;
        }

        public static IFluentAspectBuilder UseInterceptorResolver<TResolver>(this IFluentAspectBuilder builder) where TResolver : class, IInterceptorResolver
        {
            builder.Services.AddSingleton<IInterceptorResolver, TResolver>();
            return builder;
        }
    }
}
