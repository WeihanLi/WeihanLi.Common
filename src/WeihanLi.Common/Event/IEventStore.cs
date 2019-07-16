using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public interface IEventStore
    {
        /// <summary>
        /// IsEmpty
        /// </summary>
        bool IsEmpty { get; }

        bool AddSubscription<TEvent, TEventHandler>()
           where TEvent : EventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool RemoveSubscription<TEvent, TEventHandler>()
           where TEvent : EventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase;

        ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : EventBase;

        bool Clear();

        string GetEventKey<TEvent>();
    }
}
