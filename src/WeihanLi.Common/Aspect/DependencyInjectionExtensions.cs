using Microsoft.Extensions.DependencyInjection;
using System;

namespace WeihanLi.Common.Aspect
{
    public static class DependencyInjectionExtensions
    {
        public static IFluentAspectBuilder AddFluentAspects(this IServiceCollection serviceCollection, Action<FluentAspectOptions> optionsAction)
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

        public static IFluentAspectBuilder AddFluentAspects(this IServiceCollection serviceCollection)
        {
            if (null == serviceCollection)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddSingleton<IProxyTypeFactory, DefaultProxyTypeFactory>();
            serviceCollection.AddSingleton<IProxyFactory, DefaultProxyFactory>();
            serviceCollection.AddSingleton<IInterceptorResolver, FluentConfigInterceptorResolver>();

            return new FluentAspectBuilder(serviceCollection);
        }

        public static IServiceCollection AddProxyService<TService, TImplement>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
            where TImplement : TService
        {
            var serviceType = typeof(TService);
            serviceCollection.Add(new ServiceDescriptor(serviceType, sp =>
            {
                var proxyService = sp.GetRequiredService<IProxyFactory>()
                    .CreateProxy<TService, TImplement>();
                return proxyService;
            }, serviceLifetime));
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddScopedProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddTransientProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Transient);
        }

        public static IServiceCollection AddProxyService<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            var serviceType = typeof(TService);
            serviceCollection.Add(new ServiceDescriptor(serviceType, sp =>
            {
                var proxyServiceType = sp.GetRequiredService<IProxyTypeFactory>()
                    .CreateProxyType(serviceType);
                return proxyServiceType;
            }, serviceLifetime));
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonProxy<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddScopedProxy<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddTransientProxy<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Transient);
        }
    }
}
