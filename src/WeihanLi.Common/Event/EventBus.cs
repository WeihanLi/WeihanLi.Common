// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

/// <summary>
/// EventBus in process
/// </summary>
public sealed class EventBus(IEventSubscriptionManager? subscriptionManager = null) : IEventBus
{
    private static readonly ILogHelperLogger Logger = Helpers.LogHelper.GetLogger<EventBus>();

    private readonly IEventSubscriptionManager _subscriptionManager = subscriptionManager ?? new InMemoryEventSubscriptionManager();

    public async Task<bool> PublishAsync<TEvent>(TEvent @event, EventProperties? properties = null)
    {
        properties ??= new();
        if (string.IsNullOrEmpty(properties.EventId))
        {
            properties.EventId = Guid.NewGuid().ToString();
        }
        if (properties.EventAt == default)
        {
            properties.EventAt = DateTimeOffset.Now;
        }
        using var activity = DiagnosticHelper.ActivitySource.StartActivity();
        if (string.IsNullOrEmpty(properties.TraceId) && Activity.Current != null)
        {
            properties.TraceId = Activity.Current.TraceId.ToString();
        }
        var handlers = _subscriptionManager.GetEventHandlers<TEvent>();
        if (handlers.Count > 0)
        {
            var handlerTasks = new Task[handlers.Count];
            handlers.ForEach((handler, index) =>
            {
                handlerTasks[index] = handler.Handle(@event, properties).ContinueWith(r =>
                {
                    Logger.Error(r.Exception,
                        $"handle event [{typeof(TEvent).FullName}] error, eventHandlerType:{handler.GetType().FullName}");
                }, TaskContinuationOptions.OnlyOnFaulted);
            });
            await handlerTasks.WhenAllSafely().ConfigureAwait(false);

            return true;
        }
        return false;
    }

    public Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType) => _subscriptionManager.SubscribeAsync(eventType, eventHandlerType);
    public Task<bool> SubscribeAsync<TEvent>(IEventHandler<TEvent> eventHandler) => _subscriptionManager.SubscribeAsync(eventHandler);
    public Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType) => _subscriptionManager.UnSubscribeAsync(eventType, eventHandlerType);
}
