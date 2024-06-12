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
    /// <returns>whether the operation success</returns>
    [Obsolete("Use SubscribeAsync instead", true)]
    public static bool Subscribe<TEvent>(this IEventSubscriber subscriber, Func<TEvent, Task> handler)
    {
        return subscriber.SubscribeAsync(new DelegateEventHandler<TEvent>(handler))
            .GetAwaiter().GetResult();
    }

    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
    [Obsolete("Use SubscribeAsync instead", true)]
    public static bool Subscribe<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        return subscriber.Subscribe(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <param name="subscriber">event subscriber</param>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    [Obsolete("Use SubscribeAsync instead", true)]
    public static bool Subscribe(this IEventSubscriber subscriber, Type eventType, Type eventHandlerType)
    {
        return subscriber.SubscribeAsync(eventType, eventHandlerType).GetAwaiter().GetResult();
    }

    /// <summary>
    /// add event handler for event
    /// </summary>
    /// <typeparam name="TEvent">TEvent</typeparam>
    /// <typeparam name="TEventHandler">TEventHandler</typeparam>
    /// <returns>whether the operation success</returns>
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
    [Obsolete("Use UnSubscribeAsync instead", true)]
    public static bool UnSubscribe<TEvent, TEventHandler>(this IEventSubscriber subscriber)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        return subscriber.UnSubscribe(typeof(TEvent), typeof(TEventHandler));
    }

    /// <summary>
    /// remove event handler for event
    /// </summary>
    /// <param name="subscriber">event subscriber</param>
    /// <param name="eventType">event type</param>
    /// <param name="eventHandlerType">eventHandler type</param>
    /// <returns>whether the operation success</returns>
    [Obsolete("Use UnSubscribeAsync instead", true)]
    public static bool UnSubscribe(this IEventSubscriber subscriber, Type eventType, Type eventHandlerType)
    {
        return subscriber.UnSubscribeAsync(eventType, eventHandlerType).GetAwaiter().GetResult();
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
