using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public class EventStoreInMemory : IEventStore
    {
        private readonly ConcurrentDictionary<string, HashSet<Type>> _eventHandlers = new ConcurrentDictionary<string, HashSet<Type>>();

        public bool IsEmpty => _eventHandlers.Count == 0;

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].Add(typeof(TEventHandler));
            }
            else
            {
                return _eventHandlers.TryAdd(eventKey, new HashSet<Type>()
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

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEventBase
        {
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

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return _eventHandlers.ContainsKey(eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (_eventHandlers.ContainsKey(eventKey))
            {
                return _eventHandlers[eventKey].Remove(typeof(TEventHandler));
            }
            return false;
        }
    }
}
