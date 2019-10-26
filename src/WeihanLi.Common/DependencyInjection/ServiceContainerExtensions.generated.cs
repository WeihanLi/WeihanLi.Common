
using JetBrains.Annotations;
using System;

namespace WeihanLi.Common.DependencyInjection
{
    public static partial class ServiceContainerExtensions
    {
public static IServiceContainer AddSingleton([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
            return serviceContainer;
        }

public static IServiceContainer AddSingleton([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
            return serviceContainer;
        }

public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService>());
            return serviceContainer;
        }


        public static IServiceContainer AddSingleton<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService, TServiceImplement>());
            return serviceContainer;
        }

public static IServiceContainer AddScoped([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
            return serviceContainer;
        }

public static IServiceContainer AddScoped([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
            return serviceContainer;
        }

public static IServiceContainer AddScoped<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer AddScoped<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService>());
            return serviceContainer;
        }


        public static IServiceContainer AddScoped<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService, TServiceImplement>());
            return serviceContainer;
        }

public static IServiceContainer AddTransient([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
            return serviceContainer;
        }

public static IServiceContainer AddTransient([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
            return serviceContainer;
        }

public static IServiceContainer AddTransient<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer AddTransient<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService>());
            return serviceContainer;
        }


        public static IServiceContainer AddTransient<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService, TServiceImplement>());
            return serviceContainer;
        }

public static IServiceContainer TryAddSingleton([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
            return serviceContainer;
        }

public static IServiceContainer TryAddSingleton([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
            return serviceContainer;
        }

public static IServiceContainer TryAddSingleton<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.TryAdd(ServiceDefinition.Singleton<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer TryAddSingleton<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.TryAdd(ServiceDefinition.Singleton<TService>());
            return serviceContainer;
        }


        public static IServiceContainer TryAddSingleton<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.TryAdd(ServiceDefinition.Singleton<TService, TServiceImplement>());
            return serviceContainer;
        }

public static IServiceContainer TryAddScoped([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
            return serviceContainer;
        }

public static IServiceContainer TryAddScoped([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
            return serviceContainer;
        }

public static IServiceContainer TryAddScoped<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.TryAdd(ServiceDefinition.Scoped<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer TryAddScoped<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.TryAdd(ServiceDefinition.Scoped<TService>());
            return serviceContainer;
        }


        public static IServiceContainer TryAddScoped<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.TryAdd(ServiceDefinition.Scoped<TService, TServiceImplement>());
            return serviceContainer;
        }

public static IServiceContainer TryAddTransient([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
            return serviceContainer;
        }

public static IServiceContainer TryAddTransient([NotNull]this IServiceContainer serviceContainer, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainer.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
            return serviceContainer;
        }

public static IServiceContainer TryAddTransient<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.TryAdd(ServiceDefinition.Transient<TService>(func));
            return serviceContainer;
        }


        public static IServiceContainer TryAddTransient<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.TryAdd(ServiceDefinition.Transient<TService>());
            return serviceContainer;
        }


        public static IServiceContainer TryAddTransient<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.TryAdd(ServiceDefinition.Transient<TService, TServiceImplement>());
            return serviceContainer;
        }

    }
}
