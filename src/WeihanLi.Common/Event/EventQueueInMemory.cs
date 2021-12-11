using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event;

public sealed class EventQueueInMemory : IEventQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<IEventBase>> _eventQueues = new();

    public ICollection<string> GetQueues() => _eventQueues.Keys;

    public Task<ICollection<string>> GetQueuesAsync() => Task.FromResult(GetQueues());

    public bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : class, IEventBase
    {
        var queue = _eventQueues.GetOrAdd(queueName, _ => new ConcurrentQueue<IEventBase>());
        queue.Enqueue(@event);
        return true;
    }

    public Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event) where TEvent : class, IEventBase => Task.FromResult(Enqueue(queueName, @event));

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
