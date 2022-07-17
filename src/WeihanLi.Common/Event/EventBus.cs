// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

/// <summary>
/// EventBus in process
/// </summary>
public sealed class EventBus : IEventBus
{
    private static readonly ILogHelperLogger Logger = Helpers.LogHelper.GetLogger<EventBus>();

    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly IEventHandlerFactory _eventHandlerFactory;

    public EventBus(IEventSubscriptionManager subscriptionManager, IEventHandlerFactory eventHandlerFactory)
    {
        _subscriptionManager = subscriptionManager;
        _eventHandlerFactory = eventHandlerFactory;
    }

    public bool Publish<TEvent>(TEvent @event) where TEvent : class, IEventBase
    {
        var handlers = _eventHandlerFactory.GetHandlers<TEvent>();
        if (handlers.Count > 0)
        {
            var handlerTasks = new Task[handlers.Count];

            handlers.ForEach((handler, index) =>
            {
                handlerTasks[index] = handler.Handle(@event).ContinueWith(r =>
                {
                    if (r.IsFaulted)
                    {
                        Logger.Error(r.Exception?.Unwrap(),
                            $"handle event [{typeof(TEvent).FullName}] error, eventHandlerType:{handler.GetType().FullName}");
                    }
                });
            });

            _ = handlerTasks.WhenAll().ConfigureAwait(false);

            return true;
        }
        return false;
    }

    public async Task<bool> PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventBase
    {
        var handlers = _eventHandlerFactory.GetHandlers<TEvent>();
        if (handlers.Count > 0)
        {
            var handlerTasks = new Task[handlers.Count];

            handlers.ForEach((handler, index) =>
            {
                handlerTasks[index] = handler.Handle(@event).ContinueWith(r =>
                {
                    if (r.IsFaulted)
                    {
                        Logger.Error(r.Exception?.Unwrap(),
                            $"handle event [{typeof(TEvent).FullName}] error, eventHandlerType:{handler.GetType().FullName}");
                    }
                });
            });

            await handlerTasks.WhenAll().ConfigureAwait(false);

            return true;
        }
        return false;
    }

    public bool Subscribe(Type eventType, Type eventHandlerType) => _subscriptionManager.Subscribe(eventType, eventHandlerType);

    public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType) => _subscriptionManager.SubscribeAsync(eventType, eventHandlerType);

    public bool UnSubscribe(Type eventType, Type eventHandlerType) => _subscriptionManager.UnSubscribe(eventType, eventHandlerType);

    public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType) => _subscriptionManager.UnSubscribeAsync(eventType, eventHandlerType);
}
