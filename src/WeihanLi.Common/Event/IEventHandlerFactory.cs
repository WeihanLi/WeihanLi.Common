// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public interface IEventHandlerFactory
{
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    ICollection<IEventHandler> GetHandlers(Type eventType);
}

public static class EventHandlerFactoryExtensions
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static ICollection<IEventHandler<TEvent>> GetHandlers<TEvent>(this IEventHandlerFactory eventHandlerFactory)
    {
        return eventHandlerFactory.GetHandlers(typeof(TEvent))
            .Cast<IEventHandler<TEvent>>()
            .ToArray();
    }
}
