// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;
using System.Diagnostics;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event;

public class EventQueuePublisher(IEventQueue eventQueue, IOptions<EventQueuePublisherOptions> options) : IEventPublisher
{
    private readonly IEventQueue _eventQueue = eventQueue;
    private readonly EventQueuePublisherOptions _options = options.Value;

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public virtual Task<bool> PublishAsync<TEvent>(TEvent @event, EventProperties? properties)
    {
        Guard.NotNull(@event);

        properties ??= new();
        if (string.IsNullOrEmpty(properties.EventId))
        {
            properties.EventId = Guid.NewGuid().ToString();
        }
        if (properties.EventAt == default)
        {
            properties.EventAt = DateTimeOffset.Now;
        }
        using var activity = DiagnosticHelper.ActivitySource.StartActivity();
        if (string.IsNullOrEmpty(properties.TraceId) && Activity.Current != null)
        {
            properties.TraceId = Activity.Current.TraceId.ToString();
        }

        var queueName = _options.EventQueueNameResolver.Invoke(@event.GetType()) ?? "events";
        return _eventQueue.EnqueueAsync(queueName, @event, properties);
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
