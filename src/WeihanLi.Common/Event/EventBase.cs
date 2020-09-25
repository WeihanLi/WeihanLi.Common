using Newtonsoft.Json;
using System;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
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
        // ReSharper disable once PublicConstructorInAbstractClass
        protected EventBase(string eventId, DateTimeOffset eventAt)
        {
            EventId = eventId;
            EventAt = eventAt;
        }
    }

    public static class EventBaseExtensions
    {
        private static readonly JsonSerializerSettings _eventSerializerSettings = JsonSerializeExtension.SerializerSettingsWith(s =>
           {
               s.TypeNameHandling = TypeNameHandling.Objects;
           });

        public static string ToEventMsg<TEvent>(this TEvent @event) where TEvent : class, IEventBase
        {
            if (null == @event)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            return @event.ToJson(_eventSerializerSettings);
        }

        public static IEventBase ToEvent(this string eventMsg)
        {
            if (null == eventMsg)
            {
                throw new ArgumentNullException(nameof(eventMsg));
            }

            return eventMsg.JsonToObject<IEventBase>(_eventSerializerSettings);
        }
    }
}
