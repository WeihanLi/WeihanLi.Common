﻿// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventHandlerFactory
{
    ICollection<IEventHandler> GetHandlers(Type eventType);
}

public static class EventHandlerFactoryExtensions
{
    public static ICollection<IEventHandler<TEvent>> GetHandlers<TEvent>(this IEventHandlerFactory eventHandlerFactory)
        where TEvent : class, IEventBase
    {
        return eventHandlerFactory.GetHandlers(typeof(TEvent))
            .Cast<IEventHandler<TEvent>>()
            .ToArray();
    }
}
