// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;

namespace WeihanLi.Common.Event;

public sealed class EventQueueInMemory : IEventQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<IEventBase>> _eventQueues = new();

    public ICollection<string> GetQueues() => _eventQueues.Keys;

    public Task<ICollection<string>> GetQueuesAsync() => Task.FromResult(GetQueues());

    public bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : class
    {
        var internalEvent = @event switch
        {
            IEventBase eventBase => new EventWrapper<TEvent>()
            {
                Data = @event,
                Properties = new EventProperties
                {
                    EventId = eventBase.EventId, 
                    EventAt = eventBase.EventAt
                }
            },
            _ => new EventWrapper<TEvent>()
            {
                Data = @event,
                Properties = new EventProperties
                {
                    EventId = Guid.NewGuid().ToString(), 
                    EventAt = DateTimeOffset.UtcNow
                }
            }
        };
        var queue = _eventQueues.GetOrAdd(queueName, _ => new ConcurrentQueue<IEventBase>());
        queue.Enqueue(internalEvent);
        return true;
    }

    public Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event) where TEvent : class => Task.FromResult(Enqueue(queueName, @event));

    public IEventBase? Dequeue(string queueName)
    {
        if (_eventQueues.TryGetValue(queueName, out var queue))
        {
            queue.TryDequeue(out var @event);
            return @event;
        }

        return null;
    }

    public Task<IEventBase?> DequeueAsync(string queueName)
    {
        return Task.FromResult(Dequeue(queueName));
    }

    public bool TryRemoveQueue(string queueName)
    {
        return _eventQueues.TryRemove(queueName, out _);
    }
}
