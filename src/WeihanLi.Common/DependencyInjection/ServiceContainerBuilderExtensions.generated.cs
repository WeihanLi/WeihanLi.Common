using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.DependencyInjection;

public static partial class ServiceContainerBuilderExtensions
{

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingleton(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingleton<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Singleton<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScoped(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScoped(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScoped<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Scoped<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransient(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.Add(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransient<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.Add(ServiceDefinition.Transient<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddSingleton(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddSingleton(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Singleton));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddSingleton<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Singleton<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddScoped(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddScoped(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Scoped));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddScoped<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Scoped<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddTransient(this IServiceContainerBuilder serviceContainerBuilder, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]Type serviceType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddTransient(this IServiceContainerBuilder serviceContainerBuilder, Type serviceType, Type implementType)
    {
        serviceContainerBuilder.TryAdd(new ServiceDefinition(serviceType, implementType, ServiceLifetime.Transient));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder, Func<IServiceProvider, object> func)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>(func));
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TService>(this IServiceContainerBuilder serviceContainerBuilder)
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService>());
        return serviceContainerBuilder;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder TryAddTransient<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TServiceImplement>(this IServiceContainerBuilder serviceContainerBuilder) where TServiceImplement : TService
    {
        serviceContainerBuilder.TryAdd(ServiceDefinition.Transient<TService, TServiceImplement>());
        return serviceContainerBuilder;
    }

}
