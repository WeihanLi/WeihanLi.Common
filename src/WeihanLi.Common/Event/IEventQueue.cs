// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventQueue
{
    Task<ICollection<string>> GetQueuesAsync();

    Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event)
        where TEvent : class, IEventBase;

    Task<IEventBase?> DequeueAsync(string queueName);
}

public static class EventQueueExtensions
{
    private const string DefaultQueueName = "events";

    public static Task<bool> EnqueueAsync<TEvent>(this IEventQueue eventQueue, TEvent @event)
        where TEvent : class, IEventBase
    {
        return eventQueue.EnqueueAsync(DefaultQueueName, @event);
    }

    public static Task<IEventBase?> DequeueAsync(this IEventQueue eventQueue)
    {
        return eventQueue.DequeueAsync(DefaultQueueName);
    }
}
