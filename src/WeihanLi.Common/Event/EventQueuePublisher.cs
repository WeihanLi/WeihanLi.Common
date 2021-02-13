using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public class EventQueuePublisher : IEventPublisher
    {
        private readonly IEventQueue _eventQueue;

        private readonly EventQueuePublisherOptions _options;

        public EventQueuePublisher(IEventQueue eventQueue, IOptions<EventQueuePublisherOptions> options)
        {
            _eventQueue = eventQueue;
            _options = options.Value;
        }

        public virtual bool Publish<TEvent>(TEvent @event)
            where TEvent : class, IEventBase
        {
            var queueName = _options.EventQueueNameResolver.Invoke(@event.GetType()) ?? "events";

            return _eventQueue.Enqueue(queueName, @event);
        }

        public virtual Task<bool> PublishAsync<TEvent>(TEvent @event)
            where TEvent : class, IEventBase
        {
            var queueName = _options.EventQueueNameResolver.Invoke(@event.GetType()) ?? "events";
            return _eventQueue.EnqueueAsync(queueName, @event);
        }
    }

    public class EventQueuePublisherOptions
    {
        private Func<Type, string> _eventQueueNameResolver = _ => "events";

        public Func<Type, string> EventQueueNameResolver
        {
            get => _eventQueueNameResolver;
            set => _eventQueueNameResolver = Guard.NotNull(value, nameof(value));
        }
    }
}
