using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public interface IEventStore
    {
        bool AddSubscription<TEvent, TEventHandler>()
           where TEvent : IEventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool RemoveSubscription<TEvent, TEventHandler>()
           where TEvent : IEventBase
           where TEventHandler : IEventHandler<TEvent>;

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEventBase;

        ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEventBase;

        bool Clear();

        string GetEventKey<TEvent>();
    }
}
