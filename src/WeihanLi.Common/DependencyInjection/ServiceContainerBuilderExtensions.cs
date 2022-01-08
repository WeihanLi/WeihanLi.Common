using System.Reflection;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.DependencyInjection;

public static partial class ServiceContainerBuilderExtensions
{
    public static IServiceContainerBuilder AddSingleton<TService>(this IServiceContainerBuilder serviceContainerBuilder, TService service)
    {
        Guard.NotNull(service, nameof(service));
        serviceContainerBuilder.Add(new ServiceDefinition(service!, typeof(TService)));
        return serviceContainerBuilder;
    }

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypes(this IServiceContainerBuilder services, params Assembly[] assemblies)
        => RegisterAssemblyTypes(services, null, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypes(this IServiceContainerBuilder services,
        ServiceLifetime serviceLifetime, params Assembly[] assemblies)
        => RegisterAssemblyTypes(services, null, serviceLifetime, assemblies);

    /// <summary>
    /// RegisterAssemblyTypes
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypes(this IServiceContainerBuilder services,
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
    public static IServiceContainerBuilder RegisterAssemblyTypes(this IServiceContainerBuilder services, Func<Type, bool>? typesFilter, ServiceLifetime serviceLifetime, params Assembly[] assemblies)
    {
        Guard.NotNull(assemblies, nameof(assemblies));

        if (assemblies.Length == 0)
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
            services.Add(new ServiceDefinition(type, type, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypesAsImplementedInterfaces(this IServiceContainerBuilder services,
        params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: null, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypesAsImplementedInterfaces(this IServiceContainerBuilder services,
        ServiceLifetime serviceLifetime, params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: null, serviceLifetime, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces, singleton by default
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypesAsImplementedInterfaces(this IServiceContainerBuilder services, Func<Type, bool>? typesFilter, params Assembly[] assemblies)
        => RegisterAssemblyTypesAsImplementedInterfaces(services, typesFilter: typesFilter, ServiceLifetime.Singleton, assemblies);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="typesFilter">filter types to register</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <param name="assemblies">assemblies</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterAssemblyTypesAsImplementedInterfaces(
        this IServiceContainerBuilder services, Func<Type, bool>? typesFilter,
        ServiceLifetime serviceLifetime, params Assembly[] assemblies)
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
    public static IServiceContainerBuilder RegisterAssemblyTypesAsImplementedInterfaces(this IServiceContainerBuilder services, Func<Type, bool>? typesFilter, Func<Type, bool>? interfaceTypeFilter, ServiceLifetime serviceLifetime, params Assembly[] assemblies)
    {
        Guard.NotNull(assemblies, nameof(assemblies));
        if (assemblies.Length == 0)
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
                    services.Add(new ServiceDefinition(implementedInterface, type, serviceLifetime));
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
    public static IServiceContainerBuilder RegisterTypeAsImplementedInterfaces(
        this IServiceContainerBuilder services, Type type,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        => RegisterTypeAsImplementedInterfaces(services, type, null, serviceLifetime);

    /// <summary>
    /// RegisterTypeAsImplementedInterfaces
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="type">type</param>
    /// <param name="interfaceTypeFilter">interfaceTypeFilter</param>
    /// <param name="serviceLifetime">service lifetime</param>
    /// <returns>services</returns>
    public static IServiceContainerBuilder RegisterTypeAsImplementedInterfaces(this IServiceContainerBuilder services, Type type, Func<Type, bool>? interfaceTypeFilter, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        Guard.NotNull(type);
        foreach (var interfaceType in type.GetImplementedInterfaces())
        {
            if (interfaceTypeFilter?.Invoke(interfaceType) != false)
            {
                services.Add(new ServiceDefinition(interfaceType, type, serviceLifetime));
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
    public static IServiceContainerBuilder RegisterModule<TServiceModule>(this IServiceContainerBuilder services, TServiceModule module)
        where TServiceModule : IServiceContainerModule
    {
        Guard.NotNull(module, nameof(module));
        module.ConfigureServices(services);
        return services;
    }

    public static IServiceContainerBuilder RegisterAssemblyModules(
        this IServiceContainerBuilder serviceContainerBuilder, params Assembly[] assemblies)
    {
        Guard.NotNull(assemblies, nameof(assemblies));

        if (assemblies.Length == 0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        foreach (var type in assemblies.
            SelectMany(ass => ass.GetExportedTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IServiceContainerModule).IsAssignableFrom(t))
        )
        {
            try
            {
                if (Activator.CreateInstance(type) is IServiceContainerModule module)
                {
                    module.ConfigureServices(serviceContainerBuilder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        return serviceContainerBuilder;
    }
}
