
using JetBrains.Annotations;
using System;

namespace WeihanLi.Common.DependencyInjection
{
    public static partial class ServiceContainerBuilderExtensions
    {
public static IServiceContainerBuilder AddSingleton([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddSingleton([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddSingleton<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddSingleton<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddSingleton<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddScoped([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddScoped([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddScoped<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddScoped<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddScoped<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddTransient([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddTransient([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder AddTransient<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddTransient<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder AddTransient<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.Add(ServiceDefinition.Transient<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddSingleton([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddSingleton([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddSingleton<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddSingleton<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddSingleton<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddScoped([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddScoped([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddScoped<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddScoped<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddScoped<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddTransient([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddTransient([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Type serviceType, [NotNull]Type implementType)
        {
            serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
            return serviceContainerBuilder;
        }

public static IServiceContainerBuilder TryAddTransient<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>(func));
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddTransient<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>());
            return serviceContainerBuilder;
        }


        public static IServiceContainerBuilder TryAddTransient<TService, TServiceImplement>([NotNull]this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
        {
            serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService, TServiceImplement>());
            return serviceContainerBuilder;
        }

    }
}
