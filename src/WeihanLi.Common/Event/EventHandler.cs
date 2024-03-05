// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Newtonsoft.Json.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public interface IEventHandler
{
    Task Handle(object eventData);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class
{
    /// <summary>
    /// Handler event
    /// </summary>
    /// <param name="event">event</param>
    /// <param name="eventProperties">eventProperties</param>
    Task Handle(TEvent @event, EventProperties eventProperties);
}

public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : class
{
    public abstract Task Handle(TEvent @event, EventProperties eventProperties);
    
    private Task Handle(TEvent @event)
    {
        return Handle(@event, @event is IEventBase eventBase ? 
            new EventProperties
            {
                EventId = eventBase.EventId,
                EventAt = eventBase.EventAt,
            }:
            new EventProperties
            {
                EventId = Guid.NewGuid().ToString(),
                EventAt = DateTimeOffset.UtcNow
            });
    }

    public virtual Task Handle(object eventData)
    {
        Guard.NotNull(eventData);

        switch (eventData)
        {
            case TEvent data:
                return Handle(data);
            
            case JObject jObject:
                {
                    var eventWrapper = jObject.ToObject<EventWrapper<TEvent>>();
                    if (eventWrapper != null)
                        return Handle(eventWrapper.Data, eventWrapper.Properties);
                    
                    var @event = jObject.ToObject<TEvent>();
                    if (@event != null)
                        return Handle(@event);
                    
                    break;
                }
            case string eventDataJson:
                return Handle(eventDataJson.JsonToObject<TEvent>());
        }

        throw new ArgumentException(@$"Unsupported event DataType:{eventData.GetType()}", nameof(eventData));
        
        
        
    }
}
