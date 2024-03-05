// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventSubscriber
{
    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    bool Subscribe(Type eventType, Type eventHandlerType);

    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType);

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    bool UnSubscribe(Type eventType, Type eventHandlerType);

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
    public static bool Subscribe<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
        where TEvent : class
    {
        return subscriber.Subscribe(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    public static Task<bool> SubscribeAsync<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
        where TEvent : class, IEventBase
    {
        return subscriber.SubscribeAsync(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    public static bool UnSubscribe<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
        where TEvent : class, IEventBase
    {
        return subscriber.UnSubscribe(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    public static Task<bool> UnSubscribeAsync<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
        where TEvent : class, IEventBase
    {
        return subscriber.UnSubscribeAsync(typeof(TEvent), typeof(TEventHandler));
    }
}
