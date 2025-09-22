// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventHandlerFactory
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    ICollection<IEventHandler> GetHandlers(Type eventType);
}

public static class EventHandlerFactoryExtensions
{
    public static ICollection<IEventHandler<TEvent>> GetHandlers<TEvent>(this IEventHandlerFactory eventHandlerFactory)
    {
        return eventHandlerFactory.GetHandlers(typeof(TEvent))
            .Cast<IEventHandler<TEvent>>()
            .ToArray();
    }
}
