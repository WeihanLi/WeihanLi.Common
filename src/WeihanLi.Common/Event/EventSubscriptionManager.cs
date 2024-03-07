// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event;

public interface IEventSubscriptionManager : IEventSubscriber
{
    /// <summary>
    /// Get EventHandlers for event
    /// </summary>
    /// <param name="eventType">event</param>
    /// <returns>event handlers types</returns>
    ICollection<IEventHandler> GetEventHandlers(Type eventType);
}

public sealed class EventSubscriptionManagerInMemory : IEventSubscriptionManager
{
    private readonly ConcurrentDictionary<Type, ConcurrentSet<IEventHandler>> _eventHandlers = new();

    public bool Subscribe(Type eventType, Type eventHandlerType)
    {
        var handlers = _eventHandlers.GetOrAdd(eventType, []);
        return handlers.TryAdd((IEventHandler)DependencyResolver.Current.GetService(eventHandlerType));
    }

    public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType)
    {
        return Task.FromResult(Subscribe(eventType, eventHandlerType));
    }

    public Task<bool> SubscribeAsync<TEvent>(IEventHandler<TEvent> eventHandler)
    {
        var handlers = _eventHandlers.GetOrAdd(typeof(TEvent), []);
        return Task.FromResult<bool>(handlers.TryAdd(eventHandler));
    }

    public bool UnSubscribe(Type eventType, Type eventHandlerType)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            var handler = handlers.FirstOrDefault(h => h.GetType() == eventHandlerType);
            return handler is not null && handlers.TryRemove(handler);
        }

        return false;
    }

    public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType)
    {
        return Task.FromResult(UnSubscribe(eventType, eventHandlerType));
    }

    public ICollection<IEventHandler> GetEventHandlers(Type eventType)
    {
        return _eventHandlers[eventType];
    }
}

public sealed class NullEventSubscriptionManager : IEventSubscriptionManager
{
    public static readonly IEventSubscriptionManager Instance = new NullEventSubscriptionManager();

    public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(false);
    public Task<bool> SubscribeAsync<TEvent>(IEventHandler<TEvent> eventHandler) => Task.FromResult(false);

    public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType) => Task.FromResult(false);

    public ICollection<IEventHandler> GetEventHandlers(Type eventType) => Array.Empty<IEventHandler>();
}
