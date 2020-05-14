#if NETSTANDARD

using Microsoft.Extensions.DependencyInjection;

#endif

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event
{
    public interface IEventSubscriptionManager
    {
        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <returns>whether the operation success</returns>
        bool Subscribe(Type eventType, Type eventHandlerType);

        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <returns>whether the operation success</returns>
        Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType);

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <returns>whether the operation success</returns>
        bool UnSubscribe(Type eventType, Type eventHandlerType);

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <returns>whether the operation success</returns>
        Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType);

        /// <summary>
        /// Get EventHandlers for event
        /// </summary>
        /// <param name="eventType">event</param>
        /// <returns>event handlers types</returns>
        ICollection<Type> GetEventHandlerTypes(Type eventType);
    }

    public class EventSubscriptionManagerInMemory : IEventSubscriptionManager
    {
        private readonly ConcurrentDictionary<Type, ConcurrentSet<Type>> _eventHandlers = new ConcurrentDictionary<Type, ConcurrentSet<Type>>();

        public bool Subscribe(Type eventType, Type eventHandlerType)
        {
            var handlers = _eventHandlers.GetOrAdd(eventType, new ConcurrentSet<Type>());
            return handlers.TryAdd(eventHandlerType);
        }

        public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType)
        {
            return Task.FromResult(Subscribe(eventType, eventHandlerType));
        }

        public bool UnSubscribe(Type eventType, Type eventHandlerType)
        {
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                return handlers.TryRemove(eventHandlerType);
            }

            return false;
        }

        public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType)
        {
            return Task.FromResult(UnSubscribe(eventType, eventHandlerType));
        }

        public ICollection<Type> GetEventHandlerTypes(Type eventType)
        {
            return _eventHandlers[eventType];
        }
    }

#if NETSTANDARD

    public class ServiceCollectionEventSubscriptionManager : IEventSubscriptionManager
    {
        private readonly IServiceCollection _serviceCollection;

        public ServiceCollectionEventSubscriptionManager(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public bool Subscribe(Type eventType, Type eventHandlerType)
        {
            return _serviceCollection.TryAddEventHandler(eventType, eventHandlerType);
        }

        public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(Subscribe(eventType, eventHandlerType));

        public bool UnSubscribe(Type eventType, Type eventHandlerType) =>
            _serviceCollection.TryRemoveEventHandler(eventType, eventHandlerType);

        public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(UnSubscribe(eventType, eventHandlerType));

        public ICollection<Type> GetEventHandlerTypes(Type eventType)
            => _serviceCollection
                .Where(s => s.ServiceType == typeof(IEventHandler<>).MakeGenericType(eventType))
                .Select(s => s.ImplementationType)
                .ToArray()
        ;
    }

#endif

    public static class EventSubscriptionManagerExtensions
    {
        public static ICollection<Type> GetEventHandlerTypes<TEvent>(this IEventSubscriptionManager subscriptionManager)
            where TEvent : class, IEventBase
        {
            return subscriptionManager.GetEventHandlerTypes(typeof(TEvent));
        }

        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        public static bool Subscribe<TEvent, TEventHandler>(this IEventSubscriptionManager subscriptionManager)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEventBase
        {
            return subscriptionManager.Subscribe(typeof(TEvent), typeof(TEventHandler));
        }

        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        public static Task<bool> SubscribeAsync<TEvent, TEventHandler>(this IEventSubscriptionManager subscriptionManager)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEventBase
        {
            return subscriptionManager.SubscribeAsync(typeof(TEvent), typeof(TEventHandler));
        }

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        public static bool UnSubscribe<TEvent, TEventHandler>(this IEventSubscriptionManager subscriptionManager)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEventBase
        {
            return subscriptionManager.UnSubscribe(typeof(TEvent), typeof(TEventHandler));
        }

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        public static Task<bool> UnSubscribeAsync<TEvent, TEventHandler>(this IEventSubscriptionManager subscriptionManager)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEventBase
        {
            return subscriptionManager.UnSubscribeAsync(typeof(TEvent), typeof(TEventHandler));
        }
    }
}
