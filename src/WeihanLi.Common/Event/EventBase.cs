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

        public EventBase(string eventId)
        {
            EventId = eventId;
            EventAt = DateTimeOffset.UtcNow;
        }

        [JsonConstructor]
        public EventBase(string eventId, DateTimeOffset eventAt)
        {
            EventId = eventId;
            EventAt = eventAt;
        }
    }
}
