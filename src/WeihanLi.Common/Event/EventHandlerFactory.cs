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

public sealed class DependencyInjectionEventHandlerFactory(IServiceProvider? serviceProvider = null) : IEventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? DependencyResolver.Current;

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        return _serviceProvider.GetServices(eventHandlerType).Cast<IEventHandler>().ToArray();
    }
}
