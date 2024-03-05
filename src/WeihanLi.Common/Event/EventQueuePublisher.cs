// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;

namespace WeihanLi.Common.Event;

public class EventQueuePublisher(IEventQueue eventQueue, IOptions<EventQueuePublisherOptions> options) : IEventPublisher
{
    private readonly IEventQueue _eventQueue = eventQueue;

    private readonly EventQueuePublisherOptions _options = options.Value;

    public virtual Task<bool> PublishAsync<TEvent>(TEvent @event)
        where TEvent : class
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
