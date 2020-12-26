using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event
{
    public interface IEventSubscriptionManager : IEventSubscriber
    {
        /// <summary>
        /// Get EventHandlers for event
        /// </summary>
        /// <param name="eventType">event</param>
        /// <returns>event handlers types</returns>
        ICollection<Type> GetEventHandlerTypes(Type eventType);
    }

    public sealed class EventSubscriptionManagerInMemory : IEventSubscriptionManager
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

    public sealed class NullEventSubscriptionManager : IEventSubscriptionManager
    {
        public static readonly IEventSubscriptionManager Instance = new NullEventSubscriptionManager();

        public bool Subscribe(Type eventType, Type eventHandlerType) => false;

        public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(false);

        public bool UnSubscribe(Type eventType, Type eventHandlerType) => false;

        public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(false);

        public ICollection<Type> GetEventHandlerTypes(Type eventType) => Array.Empty<Type>();
    }

    public static class EventSubscriptionManagerExtensions
    {
        public static ICollection<Type> GetEventHandlerTypes<TEvent>(this IEventSubscriptionManager subscriptionManager)
            where TEvent : class, IEventBase
        {
            return subscriptionManager.GetEventHandlerTypes(typeof(TEvent));
        }
    }
}
