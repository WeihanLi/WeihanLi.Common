// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WeihanLi.Common.Event;

public interface IEventBuilder
{
    IServiceCollection Services { get; }
}

internal sealed class EventBuilder(IServiceCollection services) : IEventBuilder
{
    public IServiceCollection Services { get; } = services;
}

public static class EventBusExtensions
{
    public static IEventBuilder AddEvents(this IServiceCollection services)
    {
        services.AddOptions();

        services.TryAddSingleton<IEventHandlerFactory, DependencyInjectionEventHandlerFactory>();
        services.TryAddSingleton<IEventBus, EventBus>();

        services.TryAddSingleton<IEventQueue, EventQueueInMemory>();
        services.TryAddSingleton<IEventStore, EventStoreInMemory>();
        services.TryAddSingleton<IEventPublisher, EventQueuePublisher>();
        services.TryAddSingleton<IEventSubscriptionManager, NullEventSubscriptionManager>();

        return new EventBuilder(services);
    }

    public static IEventBuilder AddEventHandler<TEvent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEventHandler>(this IEventBuilder eventBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
      where TEvent : class, IEventBase
      where TEventHandler : class, IEventHandler<TEvent>
    {
        eventBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), serviceLifetime));
        return eventBuilder;
    }

    public static IEventBuilder AddEventHandler<TEvent>(this IEventBuilder eventBuilder, IEventHandler<TEvent> eventHandler)
        where TEvent : class, IEventBase
    {
        eventBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<TEvent>), eventHandler));
        return eventBuilder;
    }

    [RequiresUnreferencedCode("Assembly.GetTypes() requires unreferenced code")]
    public static IEventBuilder RegisterEventHandlers(this IEventBuilder builder, Func<Type, bool>? filter = null, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies)
    {
        Guard.NotNull(assemblies, nameof(assemblies));
        if (assemblies.Length == 0)
        {
            assemblies = Helpers.ReflectHelper.GetAssemblies();
        }

        var handlerTypes = assemblies
            .Select(ass => ass.GetTypes())
            .SelectMany(t => t)
            .Where(t => !t.IsAbstract && typeof(IEventHandler).IsAssignableFrom(t));
        if (filter != null)
        {
            handlerTypes = handlerTypes.Where(filter);
        }

        foreach (var handlerType in handlerTypes)
        {
            foreach (var implementedInterface in handlerType.GetTypeInfo().ImplementedInterfaces)
            {
                if (implementedInterface.IsGenericType && typeof(IEventBase).IsAssignableFrom(implementedInterface.GenericTypeArguments[0]))
                {
                    builder.Services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, handlerType, serviceLifetime));
                }
            }
        }

        return builder;
    }
}
