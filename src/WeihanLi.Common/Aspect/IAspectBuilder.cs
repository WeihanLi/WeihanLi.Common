using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect
{
    public interface IAspectBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class AspectBuilder : IAspectBuilder
    {
        public AspectBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }

        public IServiceCollection Services { get; }
    }

    public static class AspectBuilderExtensions
    {
        public static IAspectBuilder UseProxyFactory(this IAspectBuilder builder, IProxyFactory proxyFactory)
        {
            builder.Services.AddSingleton<IProxyFactory>(proxyFactory);
            return builder;
        }

        public static IAspectBuilder UseProxyTypeFactory(this IAspectBuilder builder, IProxyTypeFactory proxyTypeFactory)
        {
            builder.Services.AddSingleton<IProxyTypeFactory>(proxyTypeFactory);
            return builder;
        }

        public static IAspectBuilder UseInterceptorResolver(this IAspectBuilder builder, IInterceptorResolver resolver)
        {
            builder.Services.AddSingleton<IInterceptorResolver>(resolver);
            return builder;
        }

        public static IAspectBuilder UseProxyFactory<TProxyFactory>(this IAspectBuilder builder) where TProxyFactory : class, IProxyFactory
        {
            builder.Services.AddSingleton<IProxyFactory, TProxyFactory>();
            return builder;
        }

        public static IAspectBuilder UseProxyTypeFactory<TProxyTypeFactory>(this IAspectBuilder builder) where TProxyTypeFactory : class, IProxyTypeFactory
        {
            builder.Services.AddSingleton<IProxyTypeFactory, TProxyTypeFactory>();
            return builder;
        }

        public static IAspectBuilder UseInterceptorResolver<TResolver>(this IAspectBuilder builder) where TResolver : class, IInterceptorResolver
        {
            builder.Services.AddSingleton<IInterceptorResolver, TResolver>();
            return builder;
        }
    }
}
