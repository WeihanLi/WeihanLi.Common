using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event
{
    public class EventStoreInMemory : IEventStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentSet<Type>> _eventHandlers = new ConcurrentDictionary<string, ConcurrentSet<Type>>();

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].TryAdd(typeof(TEventHandler));
            }
            else
            {
                return _eventHandlers.TryAdd(eventKey, new ConcurrentSet<Type>()
                {
                    typeof(TEventHandler)
                });
            }
        }

        public bool Clear()
        {
            _eventHandlers.Clear();
            return true;
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase
        {
            if (_eventHandlers.Count == 0)
                return new Type[0];
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.TryGetValue(eventKey, out var handlers))
            {
                return handlers;
            }
            return new Type[0];
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : class, IEventBase
        {
            if (_eventHandlers.Count == 0)
                return false;

            var eventKey = GetEventKey<TEvent>();
            return _eventHandlers.ContainsKey(eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            if (_eventHandlers.Count == 0)
                return false;

            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].TryRemove(typeof(TEventHandler));
            }
            return false;
        }
    }
}
