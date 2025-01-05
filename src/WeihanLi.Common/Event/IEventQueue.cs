// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Runtime.CompilerServices;

namespace WeihanLi.Common.Event;

public interface IEventQueue
{
    Task<ICollection<string>> GetQueuesAsync();
    Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties? properties = null);
    Task<IEvent<TEvent>?> DequeueAsync<TEvent>(string queueName);
    IAsyncEnumerable<IEvent> ReadAllAsync(string queueName, CancellationToken cancellationToken = default);
}

public static class EventQueueExtensions
{
    private const string DefaultQueueName = "events";

    public static Task<bool> EnqueueAsync<TEvent>(this IEventQueue eventQueue, TEvent @event, EventProperties? properties = null)
        where TEvent : class
    {
        return eventQueue.EnqueueAsync(DefaultQueueName, @event, properties);
    }

    public static async IAsyncEnumerable<IEvent<TEvent>> ReadEventsAsync<TEvent>(
        this IEventQueue eventQueue,
        string queueName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
    {
        await foreach (var @event in eventQueue.ReadAllAsync(queueName, cancellationToken))
        {
            if(@event is IEvent<TEvent> eventEvent)
                yield return eventEvent;
        }
    }
}
