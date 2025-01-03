// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventQueue
{
    Task<ICollection<string>> GetQueuesAsync();
    Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties? properties = null);
    Task<IEvent<TEvent>?> DequeueAsync<TEvent>(string queueName);
    IAsyncEnumerable<IEvent<TEvent>> ReadAllEvents<TEvent>(string queueName, CancellationToken cancellationToken = default);
    IAsyncEnumerable<IEvent> ReadAllEvents(string queueName, CancellationToken cancellationToken = default);
}

public static class EventQueueExtensions
{
    private const string DefaultQueueName = "events";

    public static Task<bool> EnqueueAsync<TEvent>(this IEventQueue eventQueue, TEvent @event, EventProperties? properties = null)
        where TEvent : class
    {
        return eventQueue.EnqueueAsync(DefaultQueueName, @event, properties);
    }
}
