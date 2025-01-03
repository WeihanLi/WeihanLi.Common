// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Newtonsoft.Json;
using WeihanLi.Common.Services;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public interface IEventBase
{
    /// <summary>
    /// Event publish time
    /// </summary>
    DateTimeOffset EventAt { get; }

    /// <summary>
    /// eventId
    /// </summary>
    string EventId { get; }
}

public abstract class EventBase : IEventBase
{
    [JsonProperty]
    public DateTimeOffset EventAt { get; private set; }

    [JsonProperty]
    public string EventId { get; private set; }

    protected EventBase()
    {
        EventId = GuidIdGenerator.Instance.NewId();
        EventAt = DateTimeOffset.UtcNow;
    }

    protected EventBase(string eventId)
    {
        EventId = eventId;
        EventAt = DateTimeOffset.UtcNow;
    }

    // https://www.newtonsoft.com/json/help/html/JsonConstructorAttribute.htm
    [JsonConstructor]
#if NET
    [System.Text.Json.Serialization.JsonConstructor]
#endif
    protected EventBase(string eventId, DateTimeOffset eventAt)
    {
        EventId = eventId;
        EventAt = eventAt;
    }
}

public interface IEvent
{
    EventProperties Properties { get; }
    object? Data { get; }
}

public interface IEvent<out T>
{
    EventProperties Properties { get; }
    T Data { get; }
}

public class EventWrapper<T> : IEvent, IEvent<T>
{
    public required T Data { get; init; }
    object? IEvent.Data => (object?)Data;
    public required EventProperties Properties { get; init; }
}

public static class EventBaseExtensions
{
    private static readonly JsonSerializerSettings EventSerializerSettings = JsonSerializeExtension.SerializerSettingsWith(s =>
                {
                    s.TypeNameHandling = TypeNameHandling.Objects;
                });

    public static string ToEventMsg<TEvent>(this TEvent @event)
    {
        Guard.NotNull(@event);
        return @event.ToJson(EventSerializerSettings);
    }

    public static IEventBase ToEvent(this string eventMsg)
    {
        Guard.NotNull(eventMsg);
        return eventMsg.JsonToObject<IEventBase>(EventSerializerSettings);
    }
}
