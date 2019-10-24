using JetBrains.Annotations;
using System;

namespace WeihanLi.Common.DependencyInjection
{
    public static class ServiceContainerExtensions
    {
        public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]TService service)
        {
            serviceContainer.Add(new ServiceDefinition(service, typeof(TService)));
            return serviceContainer;
        }

        public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService>(func));
            return serviceContainer;
        }

        public static IServiceContainer AddScoped<TService>([NotNull]this IServiceContainer serviceContainer, Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService>(func));
            return serviceContainer;
        }

        public static IServiceContainer AddTransient<TService>([NotNull]this IServiceContainer serviceContainer, Func<IServiceProvider, object> func)
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService>(func));
            return serviceContainer;
        }

        public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService>());
            return serviceContainer;
        }

        public static IServiceContainer AddScoped<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService>());
            return serviceContainer;
        }

        public static IServiceContainer AddTransient<TService>([NotNull]this IServiceContainer serviceContainer)
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService>());
            return serviceContainer;
        }

        public static IServiceContainer AddSingleton<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Singleton<TService, TServiceImplement>());
            return serviceContainer;
        }

        public static IServiceContainer AddScoped<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Scoped<TService, TServiceImplement>());
            return serviceContainer;
        }

        public static IServiceContainer AddTransient<TService, TServiceImplement>([NotNull]this IServiceContainer serviceContainer) where TServiceImplement : TService
        {
            serviceContainer.Add(ServiceDefinition.Transient<TService, TServiceImplement>());
            return serviceContainer;
        }
    }
}
