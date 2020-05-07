using System;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect
{
    public static class ServiceContainerBuilderExtensions
    {
        public static IServiceContainerBuilder AddFluentAspects(this IServiceContainerBuilder serviceCollection, Action<FluentAspectOptions> optionsAction)
        {
            if (null == serviceCollection)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            if (null == optionsAction)
            {
                throw new ArgumentNullException(nameof(optionsAction));
            }
            FluentAspects.Configure(optionsAction);
            return AddFluentAspects(serviceCollection);
        }

        public static IServiceContainerBuilder AddFluentAspects(this IServiceContainerBuilder serviceCollection)
        {
            if (null == serviceCollection)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddTransient<IProxyTypeFactory, DefaultProxyTypeFactory>();
            serviceCollection.AddTransient<IProxyFactory, DefaultProxyFactory>();
            serviceCollection.AddSingleton(FluentConfigInterceptorResolver.Instance);

            return serviceCollection;
        }

        public static IServiceContainerBuilder AddProxyService<TService, TImplement>(this IServiceContainerBuilder serviceCollection, ServiceLifetime serviceLifetime)
            where TImplement : TService
            where TService : class
        {
            serviceCollection.Add(new ServiceDefinition(typeof(TService), sp =>
            {
                var proxyFactory = sp.ResolveRequiredService<IProxyFactory>();
                return proxyFactory.CreateProxy<TService, TImplement>();
            }, serviceLifetime));
            return serviceCollection;
        }

        public static IServiceContainerBuilder AddSingletonProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
            where TImplement : TService
            where TService : class
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Singleton);
        }

        public static IServiceContainerBuilder AddScopedProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
            where TImplement : TService
            where TService : class
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Scoped);
        }

        public static IServiceContainerBuilder AddTransientProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
            where TImplement : TService
            where TService : class
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Transient);
        }

        public static IServiceContainerBuilder AddProxyService<TService>(this IServiceContainerBuilder serviceCollection, ServiceLifetime serviceLifetime)
            where TService : class
        {
            serviceCollection.Add(new ServiceDefinition(typeof(TService), sp =>
            {
                var proxyFactory = sp.ResolveRequiredService<IProxyFactory>();
                return proxyFactory.CreateProxy<TService>();
            }, serviceLifetime));

            return serviceCollection;
        }

        public static IServiceContainerBuilder AddSingletonProxy<TService>(this IServiceContainerBuilder serviceCollection)
            where TService : class =>
            serviceCollection.AddProxyService<TService>(ServiceLifetime.Singleton);

        public static IServiceContainerBuilder AddScopedProxy<TService>(this IServiceContainerBuilder serviceCollection)
            where TService : class =>
            serviceCollection.AddProxyService<TService>(ServiceLifetime.Scoped);

        public static IServiceContainerBuilder AddTransientProxy<TService>(this IServiceContainerBuilder serviceCollection)
            where TService : class =>
            serviceCollection.AddProxyService<TService>(ServiceLifetime.Transient);
    }
}
