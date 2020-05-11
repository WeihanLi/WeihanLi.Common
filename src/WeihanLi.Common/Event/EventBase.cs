using Newtonsoft.Json;
using System;

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

    public interface IEventBaseWithType : IEventBase
    {
        /// <summary>
        /// Event Type
        /// </summary>
        Type Type { get; }
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
    }

    public abstract class EventBaseWithType : IEventBaseWithType
    {
        [JsonProperty]
        public DateTimeOffset EventAt { get; private set; }

        [JsonProperty]
        public string EventId { get; private set; }

        [JsonProperty]
        public Type Type { get; private set; }

        protected EventBaseWithType()
        {
            EventId = GuidIdGenerator.Instance.NewId();
            EventAt = DateTimeOffset.UtcNow;
            Type = GetType();
        }
    }
}
