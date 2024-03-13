// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public sealed class EventQueueInMemory : IEventQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<IEventWrapper>> _eventQueues = new();

    public ICollection<string> GetQueues() => _eventQueues.Keys;

    public Task<ICollection<string>> GetQueuesAsync() => Task.FromResult(GetQueues());

    public bool Enqueue<TEvent>(string queueName, TEvent @event, EventProperties? properties = null)
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
        if (string.IsNullOrEmpty(properties.TraceId) && Activity.Current != null)
        {
            properties.TraceId = Activity.Current.TraceId.ToString();
        }
        var internalEvent = new EventWrapper<TEvent>
        {
            Data = @event,
            Properties = properties
        };
        var queue = _eventQueues.GetOrAdd(queueName, _ => new ConcurrentQueue<IEventWrapper>());
        queue.Enqueue(internalEvent);
        return true;
    }

    public Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties? properties = null) => Task.FromResult(Enqueue(queueName, @event, properties));

    public Task<bool> TryDequeueAsync(string queueName, [NotNullWhen(true)]out object? @event, [NotNullWhen(true)]out EventProperties? properties)
    {
        @event = default;
        properties = default;
        
        if (_eventQueues.TryGetValue(queueName, out var queue))
        {
            if (queue.TryDequeue(out var eventWrapper))
            {
                @event = eventWrapper.Data;
                properties = eventWrapper.Properties;
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public bool TryRemoveQueue(string queueName)
    {
        return _eventQueues.TryRemove(queueName, out _);
    }
}
