// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if NET
using System.Threading.Channels;
#endif

namespace WeihanLi.Common.Event;

public sealed class EventQueueInMemory : IEventQueue
{
#if NET
    private readonly ConcurrentDictionary<string, Channel<IEvent>> _eventQueues = new();
#else
    private readonly ConcurrentDictionary<string, ConcurrentQueue<IEvent>> _eventQueues = new();
#endif
    public ICollection<string> GetQueues() => _eventQueues.Keys;

    public Task<ICollection<string>> GetQueuesAsync() => Task.FromResult(GetQueues());

    public async Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties? properties = null)
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
#if NET
        var queue = _eventQueues.GetOrAdd(queueName, _ => Channel.CreateUnbounded<IEvent>());
        await queue.Writer.WriteAsync(internalEvent);
#else
        var queue = _eventQueues.GetOrAdd(queueName, _ => new ConcurrentQueue<IEvent>());
        queue.Enqueue(internalEvent);
        await Task.CompletedTask;
#endif
        return true;
    }

    public Task<IEvent<TEvent>?> DequeueAsync<TEvent>(string queueName)
    {
        if (_eventQueues.TryGetValue(queueName, out var queue))
        {
#if NET
            if (queue.Reader.TryRead(out var eventWrapper))
#else
            if (queue.TryDequeue(out var eventWrapper))
#endif
            {
                return Task.FromResult((IEvent<TEvent>?)eventWrapper);
            }
        }

        return Task.FromResult<IEvent<TEvent>?>(null);
    }

    public async IAsyncEnumerable<IEvent> ReadAllAsync(string queueName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_eventQueues.TryGetValue(queueName, out var queue))
            {
#if NET
                await foreach (var @event in queue.Reader.ReadAllAsync(cancellationToken))
                {
                    yield return @event;
                }
#else
                while (queue.TryDequeue(out var eventWrapper))
                {
                    yield return eventWrapper;
                }
#endif
            }
            
            await Task.Delay(200, cancellationToken);
        }
    }
}
