using System;
using System.Linq;
using System.Reflection;

#if NETSTANDARD

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endif

namespace WeihanLi.Common.Event
{
#if NETSTANDARD

    public interface IEventBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class EventBuilder : IEventBuilder
    {
        public IServiceCollection Services { get; }

        public EventBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }

    public static partial class EventBusExtensions
    {
        public static IEventBuilder AddEvents(this IServiceCollection services)
        {
            services.TryAddSingleton<IEventSubscriptionManager, EventSubscriptionManagerInMemory>();
            services.TryAddSingleton<IEventBus, EventBus>();
            services.TryAddSingleton<IEventQueue, EventQueueInMemory>();
            services.TryAddSingleton<IEventStore, EventStoreInMemory>();
            services.TryAddSingleton<IEventHandlerFactory, DependencyInjectionEventHandlerFactory>();
            services.TryAddSingleton<IEventPublisher, EventQueuePublisher>();

            return new EventBuilder(services);
        }

        public static bool TryAddEventHandler(this IServiceCollection serviceCollection, Type eventType, Type eventHandlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (serviceCollection.Any(s =>
                    s.ServiceType == typeof(IEventHandler<>).MakeGenericType(eventType)
                    && s.ImplementationType == eventHandlerType
                ))
            {
                return false;
            }

            serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<>).MakeGenericType(eventType), eventHandlerType, serviceLifetime));
            return true;
        }

        public static bool TryRemoveEventHandler(this IServiceCollection serviceCollection, Type eventType, Type eventHandlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            var service = serviceCollection.FirstOrDefault(s =>
                s.ServiceType == typeof(IEventHandler<>).MakeGenericType(eventType)
                && s.ImplementationType == eventHandlerType
            );
            if (null == service)
            {
                return false;
            }

            serviceCollection.Remove(service);
            return true;
        }

        public static IServiceCollection AddEventHandler(this IServiceCollection serviceCollection, Type eventType, Type eventHandlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<>).MakeGenericType(eventType), eventHandlerType, serviceLifetime));
            return serviceCollection;
        }

        public static IServiceCollection AddEventHandler<TEvent, TEventHandler>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TEvent : class, IEventBase
            where TEventHandler : class, IEventHandler<TEvent>
        {
            serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), serviceLifetime));
            return serviceCollection;
        }

        public static IEventBuilder AddEventHandler<TEvent, TEventHandler>(this IEventBuilder eventBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
          where TEvent : class, IEventBase
          where TEventHandler : class, IEventHandler<TEvent>
        {
            eventBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), serviceLifetime));
            return eventBuilder;
        }

        public static IEventBuilder RegisterEventHandlers(this IEventBuilder builder, Func<Type, bool> filter = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
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

#endif
}
