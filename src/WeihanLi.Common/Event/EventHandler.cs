// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Newtonsoft.Json.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public interface IEventHandler
{
    Task Handle(object eventData);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class, IEventBase
{
    /// <summary>
    /// Handler event
    /// </summary>
    /// <param name="event">event</param>
    Task Handle(TEvent @event);
}

public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : class, IEventBase
{
    public abstract Task Handle(TEvent @event);

    public virtual Task Handle(object eventData)
    {
        Guard.NotNull(eventData);

        switch (eventData)
        {
            case TEvent data:
                return Handle(data);
            case JObject jObject:
                {
                    var @event = jObject.ToObject<TEvent>();
                    if (@event != null)
                    {
                        return Handle(@event);
                    }
                    break;
                }
            case string eventDataJson:
                return Handle(eventDataJson.JsonToObject<TEvent>());
        }

        throw new ArgumentException(@$"Unsupported event DataType:{eventData.GetType()}", nameof(eventData));
    }
}
