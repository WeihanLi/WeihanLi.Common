using System.Linq.Expressions;
using WeihanLi.Common.DependencyInjection;
using WeihanLi.Extensions;
using ServiceLifetime = WeihanLi.Common.DependencyInjection.ServiceLifetime;

namespace WeihanLi.Common.Aspect;

public static class ServiceContainerBuilderExtensions
{
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IFluentAspectsServiceContainerBuilder AddFluentAspects(this IServiceContainerBuilder serviceCollection, Action<FluentAspectOptions> optionsAction)
    {
        Guard.NotNull(serviceCollection);
        Guard.NotNull(optionsAction);
        FluentAspects.Configure(optionsAction);
        return AddFluentAspects(serviceCollection);
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IFluentAspectsServiceContainerBuilder AddFluentAspects(this IServiceContainerBuilder serviceCollection)
    {
        Guard.NotNull(serviceCollection);

        serviceCollection.AddTransient<IProxyTypeFactory, DefaultProxyTypeFactory>();
        serviceCollection.AddTransient<IProxyFactory, DefaultProxyFactory>();
        serviceCollection.AddSingleton(FluentConfigInterceptorResolver.Instance);

        return new FluentAspectsServiceContainerBuilder(serviceCollection);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddProxyService<TService, TImplement>(this IServiceContainerBuilder serviceCollection, ServiceLifetime serviceLifetime)
        where TImplement : TService
        where TService : class
    {
        Guard.NotNull(serviceCollection);

        serviceCollection.Add(new ServiceDefinition(typeof(TService), sp =>
        {
            var proxyFactory = sp.ResolveRequiredService<IProxyFactory>();
            return proxyFactory.CreateProxy<TService, TImplement>();
        }, serviceLifetime));
        return serviceCollection;
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingletonProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
        where TImplement : TService
        where TService : class
    {
        Guard.NotNull(serviceCollection);
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Singleton);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScopedProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
        where TImplement : TService
        where TService : class
    {
        Guard.NotNull(serviceCollection);
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Scoped);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransientProxy<TService, TImplement>(this IServiceContainerBuilder serviceCollection)
        where TImplement : TService
        where TService : class
    {
        Guard.NotNull(serviceCollection);
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Transient);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddProxyService<TService>(this IServiceContainerBuilder serviceCollection, ServiceLifetime serviceLifetime)
        where TService : class
    {
        Guard.NotNull(serviceCollection);
        serviceCollection.Add(new ServiceDefinition(typeof(TService), sp =>
        {
            var proxyFactory = sp.ResolveRequiredService<IProxyFactory>();
            return proxyFactory.CreateProxy<TService>();
        }, serviceLifetime));

        return serviceCollection;
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddSingletonProxy<TService>(this IServiceContainerBuilder serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Singleton);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddScopedProxy<TService>(this IServiceContainerBuilder serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Scoped);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainerBuilder AddTransientProxy<TService>(this IServiceContainerBuilder serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Transient);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceContainer BuildFluentAspectsContainer(this IServiceContainerBuilder serviceCollection,
        Action<FluentAspectOptions>? optionsAction,
        Action<IFluentAspectsServiceContainerBuilder>? aspectBuildAction = null,
        Expression<Func<Type, bool>>? ignoreTypesFilter = null)
    {
        Guard.NotNull(serviceCollection);
        var services = new ServiceContainerBuilder();

        var aspectBuilder = null != optionsAction
            ? serviceCollection.AddFluentAspects(optionsAction)
            : serviceCollection.AddFluentAspects();
        aspectBuildAction?.Invoke(aspectBuilder);

        Expression<Func<Type, bool>> ignoreTypesExpression = t => "WeihanLi.Common.Aspect".Equals(t.Namespace);
        if (null != ignoreTypesFilter)
        {
            ignoreTypesExpression = ignoreTypesExpression.Or(ignoreTypesFilter);
        }

        var ignoreTypesPredicate = ignoreTypesExpression.Compile();

        using (var serviceProvider = serviceCollection.Build())
        {
            var proxyTypeFactory = serviceProvider.ResolveRequiredService<IProxyTypeFactory>();

            foreach (var descriptor in serviceCollection)
            {
                if (descriptor.ServiceType.IsSealed
                    || descriptor.ServiceType.IsNotPublic
                    || descriptor.ServiceType.IsGenericTypeDefinition
                )
                {
                    services.Add(descriptor);
                    continue;
                }

                if (ignoreTypesPredicate(descriptor.ServiceType))
                {
                    services.Add(descriptor);
                    continue;
                }

                if (descriptor.ImplementType != null)
                {
                    if (descriptor.ImplementType.IsNotPublic
                        || descriptor.ImplementType.IsProxyType()
                    )
                    {
                        services.Add(descriptor);
                        continue;
                    }

                    if (descriptor.ServiceType.IsClass
                        && descriptor.ImplementType.IsSealed)
                    {
                        services.Add(descriptor);
                        continue;
                    }

                    if (descriptor.ServiceType.IsGenericTypeDefinition
                        || descriptor.ImplementType.IsGenericTypeDefinition)
                    {
                        var proxyType = proxyTypeFactory.CreateProxyType(descriptor.ServiceType, descriptor.ImplementType);
                        services.Add(new ServiceDefinition(descriptor.ServiceType, proxyType,
                            descriptor.ServiceLifetime));
                        continue;
                    }
                }

                Func<IServiceProvider, object?>? serviceFactory = null;

                if (descriptor.ImplementationInstance != null)
                {
                    if (descriptor.ImplementationInstance.GetType().IsPublic)
                    {
                        serviceFactory = provider => provider.ResolveRequiredService<IProxyFactory>()
                            .CreateProxyWithTarget(descriptor.ServiceType, descriptor.ImplementationInstance);
                    }
                }
                else if (descriptor.ImplementType != null)
                {
                    serviceFactory = provider =>
                    {
                        var proxy = provider.ResolveRequiredService<IProxyFactory>()
                            .CreateProxy(descriptor.ServiceType, descriptor.ImplementType);
                        return proxy;
                    };
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    serviceFactory = provider =>
                    {
                        var implement = descriptor.ImplementationFactory(provider);
                        if (implement == null)
                        {
                            return null;
                        }

                        var implementType = implement.GetType();
                        if (implementType.IsNotPublic
                            || implementType.IsProxyType())
                        {
                            return implement;
                        }

                        return provider.ResolveRequiredService<IProxyFactory>()
                            .CreateProxyWithTarget(descriptor.ServiceType, implement);
                    };
                }

                if (serviceFactory != null)
                {
                    services.Add(new ServiceDefinition(descriptor.ServiceType, serviceFactory!,
                        descriptor.ServiceLifetime));
                }
                else
                {
                    services.Add(descriptor);
                }
            }
        }

        var container = services.Build();
        DependencyResolver.SetDependencyResolver(container);
        return container;
    }
}
