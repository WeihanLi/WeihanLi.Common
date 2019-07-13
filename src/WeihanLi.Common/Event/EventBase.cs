using System;
using Newtonsoft.Json;

namespace WeihanLi.Common.Event
{
    public abstract class EventBase
    {
        [JsonProperty]
        public DateTimeOffset EventAt { get; private set; }

        [JsonProperty]
        public string EventId { get; private set; }

        public EventBase()
        {
            EventId = GuidIdGenerator.Instance.NewId();
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
