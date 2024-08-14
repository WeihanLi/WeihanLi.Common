
namespace WeihanLi.Common.DependencyInjection;

public static partial class ServiceContainerBuilderExtensions
{
    public static IServiceContainerBuilder AddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddSingleton<TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddSingleton<TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddSingleton<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddScoped(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddScoped(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddScoped<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddTransient<TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddTransient<TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder AddTransient<TService, TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddSingleton<TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddSingleton<TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddSingleton<TService, TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddScoped(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddScoped(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddScoped<TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddScoped<TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddScoped<TService, TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddTransient<TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>(func));
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddTransient<TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>());
        return serviceContainerBuilder;
    }

    public static IServiceContainerBuilder TryAddTransient<TService, TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

}
