// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public interface IEventSubscriber
{
    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    [RequiresUnreferencedCode("Calls WeihanLi.Common.Helpers.ActivatorHelper.GetServiceOrCreateInstance(Type)")]
    Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType);

    /// <summary>
    /// add event handler instance for event
    /// </summary>
    /// <param name="eventHandler">event handler</param>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns></returns>
    Task<bool> SubscribeAsync<TEvent>(IEventHandler<TEvent> eventHandler);

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType);
}

public static class EventSubscriberExtensions
{
    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    [RequiresUnreferencedCode("Calls WeihanLi.Common.Helpers.ActivatorHelper.GetServiceOrCreateInstance(Type)")]
    public static Task<bool> SubscribeAsync<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        return subscriber.SubscribeAsync(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    public static Task<bool> UnSubscribeAsync<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        return subscriber.UnSubscribeAsync(typeof(TEvent), typeof(TEventHandler));
    }
}
