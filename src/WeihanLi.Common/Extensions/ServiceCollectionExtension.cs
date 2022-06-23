// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceModule
{
    void ConfigureServices(IServiceCollection services);
}

public static class ServiceCollectionExtension
{
    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypes(this IServiceCollection services, params Assembly[] assemblies)
        => RegisterAssemblyTypes(services, null, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypes(this IServiceCollection services,
        ServiceLifetime serviceLifetime, params Assembly[] assemblies)
        => RegisterAssemblyTypes(services, null, serviceLifetime, assemblies);

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypes(this IServiceCollection services,
        Func<Type, bool>? typesFilter, params Assembly[] assemblies)
        => RegisterAssemblyTypes(services, typesFilter, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypes(this IServiceCollection services, Func<Type, bool>? typesFilter, ServiceLifetime serviceLifetime, params Assembly[] assemblies)
    {
        if (Guard.NotNull(assemblies, nameof(assemblies)).Length == 0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        var types = assemblies
            .Select(assembly => assembly.GetTypes())
            .SelectMany(t => t)
            .Where(t => !t.IsAbstract)
            ;
        if (typesFilter != null)
        {
            types = types.Where(typesFilter);
        }

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypesAsImplementedInterfaces(this IServiceCollection services,
        params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: null, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypesAsImplementedInterfaces(this IServiceCollection services,
        ServiceLifetime serviceLifetime, params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: null, serviceLifetime, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces, singleton by default
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypesAsImplementedInterfaces(this IServiceCollection services, Func<Type, bool> typesFilter, params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: typesFilter, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypesAsImplementedInterfaces(
        this IServiceCollection services, Func<Type, bool>? typesFilter, ServiceLifetime serviceLifetime,
        params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter, null, serviceLifetime, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="interfaceTypeFilter">filter interface types to register</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyTypesAsImplementedInterfaces(this IServiceCollection services, Func<Type, bool>? typesFilter, Func<Type, bool>? interfaceTypeFilter, ServiceLifetime serviceLifetime, params Assembly[] assemblies)
    {
        if (Guard.NotNull(assemblies, nameof(assemblies)).Length == 0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        var types = assemblies
            .Select(assembly => assembly.GetTypes())
            .SelectMany(t => t)
            .Where(t => !t.IsAbstract)
            ;
        if (typesFilter != null)
        {
            types = types.Where(typesFilter);
        }

        foreach (var type in types)
        {
            foreach (var implementedInterface in type.GetImplementedInterfaces())
            {
                if (interfaceTypeFilter?.Invoke(implementedInterface) != false)
                {
                    services.Add(new ServiceDescriptor(implementedInterface, type, serviceLifetime));
                }
            }
        }

        return services;
    }

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="type">type</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterTypeAsImplementedInterfaces(this IServiceCollection services, Type type, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        => RegisterTypeAsImplementedInterfaces(services, type, null, serviceLifetime);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="type">type</param>
    /// <param name="interfaceTypeFilter">interfaceTypeFilter</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterTypeAsImplementedInterfaces(this IServiceCollection services, Type type, Func<Type, bool>? interfaceTypeFilter, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        if (type != null)
        {
            foreach (var interfaceType in type.GetImplementedInterfaces())
            {
                if (interfaceTypeFilter?.Invoke(interfaceType) != false)
                {
                    services.Add(new ServiceDescriptor(interfaceType, type, serviceLifetime));
                }
            }
        }
        return services;
    }

    /// <summary>
    /// Register Module
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="module">service module</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterModule<TServiceModule>(this IServiceCollection services, TServiceModule module)
        where TServiceModule : IServiceModule
    {
        Guard.NotNull(module, nameof(module)).ConfigureServices(services);
        return services;
    }

    /// <summary>
    /// RegisterAssemblyModules
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceCollection RegisterAssemblyModules(
        this IServiceCollection services, params Assembly[] assemblies)
    {
        Guard.NotNull(assemblies, nameof(assemblies));
        if (assemblies.Length == 0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }
        foreach (var type in assemblies.SelectMany(ass => ass.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IServiceModule).IsAssignableFrom(t))
        )
        {
            try
            {
                if (Activator.CreateInstance(type) is IServiceModule module)
                {
                    module.ConfigureServices(services);
                }
            }
            catch (Exception e)
            {
                InvokeHelper.OnInvokeException?.Invoke(e);
            }
        }

        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TDecorator"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
         where TService : class
         where TDecorator : class, TService
    {
        var serviceType = typeof(TService);
        var service = services.FirstOrDefault(x => x.ServiceType == serviceType);
        if (service == null)
        {
            throw new InvalidOperationException("The service is not registed, service need to be registered before decorating");
        }
        // create the object factory for our decorator type
        var objectFactory = ActivatorUtilities.CreateFactory(typeof(TDecorator), new[] { serviceType });
        var decoratorService = new ServiceDescriptor(serviceType, sp => objectFactory(sp, new object?[]
        {
            sp.CreateInstance(service)
        }), service.Lifetime);

        services.Replace(decoratorService);
        return services;
    }

    public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Type implementType)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == serviceType);
        if (service == null)
        {
            throw new InvalidOperationException("The service is not registed, service need to be registered before decorating");
        }
        // create the object factory for our decorator type
        var objectFactory = ActivatorUtilities.CreateFactory(implementType, new[] { serviceType });
        var decoratorService = new ServiceDescriptor(serviceType, sp => objectFactory(sp, new object?[]
        {
            sp.CreateInstance(service)
        }), service.Lifetime);

        services.Replace(decoratorService);
        return services;
    }

    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory(services);

        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType!);
    }
}
