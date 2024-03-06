// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Newtonsoft.Json.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public interface IEventHandler
{
    Task Handle(object eventData, EventProperties properties);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class
{
    /// <summary>
    /// Handler event
    /// </summary>
    /// <param name="event">event</param>
    /// <param name="properties">eventProperties</param>
    Task Handle(TEvent @event, EventProperties properties);
}

public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : class
{
    public abstract Task Handle(TEvent @event, EventProperties eventProperties);

    public virtual Task Handle(object eventData, EventProperties properties)
    {
        Guard.NotNull(eventData);

        switch (eventData)
        {
            case TEvent data:
                return Handle(data, properties);
            
            case JObject jObject:
                {
                    var @event = jObject.ToObject<TEvent>();
                    if (@event != null)
                        return Handle(@event, properties);
                    
                    break;
                }
            case string eventDataJson:
                return Handle(eventDataJson.JsonToObject<TEvent>(), properties);
        }

        throw new ArgumentException(@$"Unsupported event DataType:{eventData.GetType()}", nameof(eventData));
    }
}
