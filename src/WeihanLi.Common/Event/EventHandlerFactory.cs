// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public sealed class DefaultEventHandlerFactory(IEventSubscriptionManager subscriptionManager) : IEventHandlerFactory
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlers = subscriptionManager.GetEventHandlers(eventType);
        return eventHandlers;
    }
}
