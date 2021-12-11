using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event;

public sealed class EventStoreInMemory : IEventStore
{
    private readonly ConcurrentDictionary<string, IEventBase> _events = new();

    public int SaveEvents(params IEventBase[] events)
    {
        if (Guard.NotNull(events, nameof(events)).Length == 0)
            return 0;

        return events.Count(@event => _events.TryAdd(@event.EventId, @event));
    }

    public Task<int> SaveEventsAsync(params IEventBase[] events) => Task.FromResult(SaveEvents(events));

    public int DeleteEvents(params string[] eventIds)
    {
        if (Guard.NotNull(eventIds, nameof(eventIds)).Length == 0)
            return 0;

        return eventIds.Count(eventId => _events.TryRemove(eventId, out _));
    }

    public Task<int> DeleteEventsAsync(params string[] eventIds) => Task.FromResult(DeleteEvents(eventIds));
}
