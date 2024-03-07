// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event;

public sealed class DefaultEventHandlerFactory(IEventSubscriptionManager subscriptionManager, IServiceProvider serviceProvider) : IEventHandlerFactory
{
    private readonly IEventSubscriptionManager _subscriptionManager = subscriptionManager;
    private readonly ConcurrentDictionary<Type, ICollection<IEventHandler>> _eventHandlers = new();
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlers = _eventHandlers.GetOrAdd(eventType, type =>
        {
            var handlers = _subscriptionManager.GetEventHandlers(type);
            return handlers;
        });
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
