// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;

namespace WeihanLi.Common.Event;

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

        return _eventQueue.EnqueueAsync(queueName, @event).ConfigureAwait(false)
            .GetAwaiter().GetResult();
    }

    public virtual Task<bool> PublishAsync<TEvent>(TEvent @event)
        where TEvent : class, IEventBase
    {
        var queueName = _options.EventQueueNameResolver.Invoke(@event.GetType()) ?? "events";
        return _eventQueue.EnqueueAsync(queueName, @event);
    }
}

public sealed class EventQueuePublisherOptions
{
    private Func<Type, string> _eventQueueNameResolver = _ => "events";

    public Func<Type, string> EventQueueNameResolver
    {
        get => _eventQueueNameResolver;
        set => _eventQueueNameResolver = Guard.NotNull(value);
    }
}
