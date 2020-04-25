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

            serviceCollection.AddTransient<IProxyTypeFactory, DefaultProxyTypeFactory>();
            serviceCollection.AddTransient<IProxyFactory, DefaultProxyFactory>();
            serviceCollection.AddSingleton<IInterceptorResolver, FluentConfigInterceptorResolver>();

            return new FluentAspectBuilder(serviceCollection);
        }

        public static IServiceCollection AddProxyService<TService, TImplement>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
            where TImplement : TService
            where TService : class
        {
            var serviceType = typeof(TService);

            serviceCollection.Add(new ServiceDescriptor(serviceType, sp => sp.GetRequiredService<IProxyFactory>()
                .CreateProxy<TService, TImplement>(), serviceLifetime));
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
            where TService : class

        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddScopedProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
            where TService : class

        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddTransientProxy<TService, TImplement>(this IServiceCollection serviceCollection)
            where TImplement : TService
            where TService : class

        {
            return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Transient);
        }

        public static IServiceCollection AddProxyService<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
            where TService : class
        {
            var serviceType = typeof(TService);

            serviceCollection.Add(new ServiceDescriptor(serviceType, sp =>
            {
                var proxyFactory = sp.GetRequiredService<IProxyFactory>();
                return proxyFactory.CreateProxy<TService>();
            }, serviceLifetime));

            return serviceCollection;
        }

        public static IServiceCollection AddSingletonProxy<TService>(this IServiceCollection serviceCollection)
            where TService : class

        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddScopedProxy<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddTransientProxy<TService>(this IServiceCollection serviceCollection)
            where TService : class

        {
            return serviceCollection.AddProxyService<TService>(ServiceLifetime.Transient);
        }
    }
}
