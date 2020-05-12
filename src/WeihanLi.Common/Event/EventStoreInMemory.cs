using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public sealed class EventStoreInMemory : IEventStore
    {
        private readonly ConcurrentDictionary<string, IEventBase> _events = new ConcurrentDictionary<string, IEventBase>();

        public int SaveEvents(params IEventBase[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            var successCount = 0;
            foreach (var @event in events)
            {
                if (_events.TryAdd(@event.EventId, @event))
                {
                    successCount++;
                }
            }
            return successCount;
        }

        public Task<int> SaveEventsAsync(params IEventBase[] events) => Task.FromResult(SaveEvents(events));

        public int DeleteEvents(params string[] eventIds)
        {
            if (null == eventIds || eventIds.Length == 0)
                return 0;

            var successCount = 0;
            foreach (var eventId in eventIds)
            {
                if (_events.TryRemove(eventId, out _))
                {
                    successCount++;
                }
            }
            return successCount;
        }

        public Task<int> DeleteEventsAsync(params string[] events) => Task.FromResult(DeleteEvents(events));
    }
}
