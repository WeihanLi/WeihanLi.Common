using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public sealed class EventQueueInMemory : IEventQueue
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<IEventBase>> _eventQueues = new ConcurrentDictionary<string, ConcurrentQueue<IEventBase>>();

        public ICollection<string> Queues => _eventQueues.Keys;

        public bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : IEventBase
        {
            var queue = _eventQueues.GetOrAdd(queueName, q => new ConcurrentQueue<IEventBase>());
            queue.Enqueue(@event);
            return true;
        }

        public bool TryDequeue(string queueName, out IEventBase @event)
        {
            var queue = _eventQueues.GetOrAdd(queueName, q => new ConcurrentQueue<IEventBase>());
            return queue.TryDequeue(out @event);
        }

        public bool TryRemoveQueue(string queueName)
        {
            return _eventQueues.TryRemove(queueName, out _);
        }
    }
}
