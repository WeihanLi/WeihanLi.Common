// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public sealed class DefaultEventHandlerFactory(IEventSubscriptionManager subscriptionManager) : IEventHandlerFactory
{
    private readonly IEventSubscriptionManager _subscriptionManager = subscriptionManager;

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlers = _subscriptionManager.GetEventHandlers(eventType);
        return eventHandlers;
    }
}
