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
            services.AddOptions();

            services.TryAddSingleton<IEventSubscriptionManager, NullEventSubscriptionManager>();
            services.TryAddSingleton<IEventHandlerFactory, DependencyInjectionEventHandlerFactory>();

            services.TryAddSingleton<IEventSubscriber, NullEventSubscriptionManager>();
            services.TryAddSingleton<IEventPublisher, EventBus>();
            services.TryAddSingleton<IEventBus, EventBus>();

            services.TryAddSingleton<IEventQueue, EventQueueInMemory>();
            services.TryAddSingleton<IEventStore, EventStoreInMemory>();

            return new EventBuilder(services);
        }

        public static IEventBuilder AddEventHandler<TEvent, TEventHandler>(this IEventBuilder eventBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
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
