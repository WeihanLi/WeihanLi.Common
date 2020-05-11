using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event
{
    public interface IEventSubscriptionManager
    {
        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Subscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        Task<bool> SubscribeAsync<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool UnSubscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        Task<bool> UnSubscribeAsync<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase;
    }

    public class EventSubscriptionManagerInMemory : IEventSubscriptionManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentSet<Type>> _eventHandlers = new ConcurrentDictionary<string, ConcurrentSet<Type>>();

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool Subscribe<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].TryAdd(typeof(TEventHandler));
            }

            return _eventHandlers.TryAdd(eventKey, new ConcurrentSet<Type>()
            {
                typeof(TEventHandler)
            });
        }

        public Task<bool> SubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent>
        {
            return Task.FromResult(Subscribe<TEvent, TEventHandler>());
        }

        public bool UnSubscribe<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].TryRemove(typeof(TEventHandler));
            }

            return false;
        }

        public Task<bool> UnSubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent>
        {
            return Task.FromResult(UnSubscribe<TEvent, TEventHandler>());
        }

        public bool HasSubscriptionForEvent<TEvent>() where TEvent : class, IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return _eventHandlers.ContainsKey(eventKey);
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return _eventHandlers[eventKey];
        }
    }
}
