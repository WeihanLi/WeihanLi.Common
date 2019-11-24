using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public interface IEventStore
    {
        bool AddSubscription<TEvent, TEventHandler>()
           where TEvent : class, IEventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool RemoveSubscription<TEvent, TEventHandler>()
           where TEvent : class, IEventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : class, IEventBase;

        ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase;

        bool Clear();

        string GetEventKey<TEvent>();
    }
}
