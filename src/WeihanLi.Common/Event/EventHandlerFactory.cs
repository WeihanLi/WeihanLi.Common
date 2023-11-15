// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event;

public sealed class DefaultEventHandlerFactory : IEventHandlerFactory
{
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ConcurrentDictionary<Type, ICollection<IEventHandler>> _eventHandlers = new();
    private readonly IServiceProvider _serviceProvider;

    public DefaultEventHandlerFactory(IEventSubscriptionManager subscriptionManager, IServiceProvider serviceProvider)
    {
        _subscriptionManager = subscriptionManager;
        _serviceProvider = serviceProvider;
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlers = _eventHandlers.GetOrAdd(eventType, type =>
        {
            var handlerTypes = _subscriptionManager.GetEventHandlerTypes(type);
            var handlers = handlerTypes
                .Select(t => (IEventHandler)_serviceProvider.GetServiceOrCreateInstance(t))
                .ToArray();
            return handlers;
        });
        return eventHandlers;
    }
}

public sealed class DependencyInjectionEventHandlerFactory : IEventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionEventHandlerFactory(IServiceProvider? serviceProvider = null)
    {
        _serviceProvider = serviceProvider ?? DependencyResolver.Current;
    }

    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public ICollection<IEventHandler> GetHandlers(Type eventType)
    {
        var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        return _serviceProvider.GetServices(eventHandlerType).Cast<IEventHandler>().ToArray();
    }
}
