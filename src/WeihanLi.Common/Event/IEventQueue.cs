// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public interface IEventQueue
{
    Task<ICollection<string>> GetQueuesAsync();

    Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties? properties = null);

    Task<bool> TryDequeueAsync(string queueName, [MaybeNullWhen(false)] out object @event, [MaybeNullWhen(false)] out EventProperties properties);

    // IAsyncEnumerable<(TEvent Event, EventProperties Properties)> ReadAllEvents<TEvent>(string queueName, CancellationToken cancellationToken = default);
}

public static class EventQueueExtensions
{
    private const string DefaultQueueName = "events";

    public static Task<bool> EnqueueAsync<TEvent>(this IEventQueue eventQueue, TEvent @event, EventProperties? properties = null)
        where TEvent : class
    {
        return eventQueue.EnqueueAsync(DefaultQueueName, @event);
    }
}
