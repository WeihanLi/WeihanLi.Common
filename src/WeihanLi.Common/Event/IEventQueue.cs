using System.Collections.Generic;

namespace WeihanLi.Common.Event
{
    public interface IEventQueue
    {
        ICollection<string> Queues { get; }

        bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : IEventBase;

        bool TryDequeue(string queueName, out IEventBase @event);

        bool TryRemoveQueue(string queueName);
    }
}
