using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public class EventQueuePublisher : IEventPublisher
    {
        private readonly IEventQueue _eventQueue;

        private readonly EventQueuePublisherOptions _options;

        public EventQueuePublisher(IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
            _options = new EventQueuePublisherOptions();
        }

        public void Config(Action<EventQueuePublisherOptions> configAction)
        {
            configAction?.Invoke(_options);
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
        private Func<Type, string> _eventQueueNameResolver = t => "events";

        public Func<Type, string> EventQueueNameResolver
        {
            get => _eventQueueNameResolver;
            set
            {
                if (null != _eventQueueNameResolver)
                {
                    _eventQueueNameResolver = value;
                }
            }
        }
    }
}
