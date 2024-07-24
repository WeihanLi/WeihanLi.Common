using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq.Expressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect;

public static class ServiceCollectionExtensions
{
    public static IFluentAspectsBuilder AddFluentAspects(this IServiceCollection serviceCollection, Action<FluentAspectOptions> optionsAction)
    {
        Guard.NotNull(serviceCollection);
        Guard.NotNull(optionsAction);
        FluentAspects.Configure(optionsAction);
        return AddFluentAspects(serviceCollection);
    }

    public static IFluentAspectsBuilder AddFluentAspects(this IServiceCollection serviceCollection)
    {
        Guard.NotNull(serviceCollection);

        serviceCollection.TryAddTransient<IProxyTypeFactory, DefaultProxyTypeFactory>();
        serviceCollection.TryAddTransient<IProxyFactory, DefaultProxyFactory>();
        serviceCollection.TryAddSingleton(FluentConfigInterceptorResolver.Instance);

        return new FluentAspectsBuilder(serviceCollection);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddProxyService<TService, TImplement>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        where TImplement : TService
        where TService : class
    {
        serviceCollection.Add(new ServiceDescriptor(typeof(TService), sp =>
        {
            var proxyFactory = sp.GetRequiredService<IProxyFactory>();
            return proxyFactory.CreateProxy<TService, TImplement>();
        }, serviceLifetime));
        return serviceCollection;
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddSingletonProxy<TService, TImplement>(this IServiceCollection serviceCollection)
        where TImplement : TService
        where TService : class
    {
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Singleton);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddScopedProxy<TService, TImplement>(this IServiceCollection serviceCollection)
        where TImplement : TService
        where TService : class
    {
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Scoped);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddTransientProxy<TService, TImplement>(this IServiceCollection serviceCollection)
        where TImplement : TService
        where TService : class
    {
        return serviceCollection.AddProxyService<TService, TImplement>(ServiceLifetime.Transient);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddProxyService<TService>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        where TService : class
    {
        serviceCollection.Add(new ServiceDescriptor(typeof(TService), sp =>
        {
            var proxyFactory = sp.GetRequiredService<IProxyFactory>();
            return proxyFactory.CreateProxy<TService>();
        }, serviceLifetime));

        return serviceCollection;
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddSingletonProxy<TService>(this IServiceCollection serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Singleton);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddScopedProxy<TService>(this IServiceCollection serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Scoped);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceCollection AddTransientProxy<TService>(this IServiceCollection serviceCollection)
        where TService : class =>
        serviceCollection.AddProxyService<TService>(ServiceLifetime.Transient);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static IServiceProvider BuildFluentAspectsProvider(this IServiceCollection serviceCollection,
        Action<FluentAspectOptions>? optionsAction,
        Action<IFluentAspectsBuilder>? aspectBuildAction = null,
        Expression<Func<Type, bool>>? ignoreTypesFilter = null,
        ServiceProviderOptions? serviceProviderOptions = null)
    {
        IServiceCollection services = new ServiceCollection();

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

        using (var serviceProvider = serviceCollection.BuildServiceProvider())
        {
            var proxyTypeFactory = serviceProvider.GetRequiredService<IProxyTypeFactory>();

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

                if (descriptor.ImplementationType != null)
                {
                    if (descriptor.ImplementationType.IsNotPublic
                        || descriptor.ImplementationType.IsProxyType()
                    )
                    {
                        services.Add(descriptor);
                        continue;
                    }

                    if (descriptor.ServiceType.IsClass
                        && descriptor.ImplementationType.IsSealed)
                    {
                        services.Add(descriptor);
                        continue;
                    }

                    if (descriptor.ServiceType.IsGenericTypeDefinition
                        || descriptor.ImplementationType.IsGenericTypeDefinition)
                    {
                        var proxyType = proxyTypeFactory.CreateProxyType(descriptor.ServiceType, descriptor.ImplementationType);
                        services.Add(new ServiceDescriptor(descriptor.ServiceType, proxyType,
                            descriptor.Lifetime));
                        continue;
                    }
                }

                Func<IServiceProvider, object>? serviceFactory = null;
                if (descriptor.ImplementationInstance != null)
                {
                    if (descriptor.ImplementationInstance.GetType().IsPublic)
                    {
                        serviceFactory = provider => provider.GetRequiredService<IProxyFactory>()
                            .CreateProxyWithTarget(descriptor.ServiceType, descriptor.ImplementationInstance);
                    }
                }
                else if (descriptor.ImplementationType != null)
                {
                    serviceFactory = provider =>
                    {
                        var proxy = provider.GetRequiredService<IProxyFactory>()
                            .CreateProxy(descriptor.ServiceType, descriptor.ImplementationType);
                        return proxy;
                    };
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    serviceFactory = provider =>
                    {
                        var implement = descriptor.ImplementationFactory(provider);
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

                if (null != serviceFactory)
                {
                    services.Add(new ServiceDescriptor(descriptor.ServiceType, serviceFactory,
                        descriptor.Lifetime));
                }
                else
                {
                    services.Add(descriptor);
                }
            }
        }

        DependencyResolver.SetDependencyResolver(services);

        return services.BuildServiceProvider(serviceProviderOptions ?? new ServiceProviderOptions());
    }
}
