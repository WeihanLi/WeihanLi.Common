// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public sealed class DefaultEventHandlerFactory(IEventSubscriptionManager subscriptionManager) : IEventHandlerFactory
{
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlers = subscriptionManager.GetEventHandlers(eventType);
        return eventHandlers;
    }
}
