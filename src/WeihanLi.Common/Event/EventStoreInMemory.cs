// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public sealed class EventStoreInMemory : IEventStore
{
    private readonly ConcurrentDictionary<string, IEventBase> _events = new();

    public int SaveEvents(ICollection<IEventBase> events)
    {
        if (events.IsNullOrEmpty())
            return 0;

        return events.Count(@event => _events.TryAdd(@event.EventId, @event));
    }

    public Task<int> SaveEventsAsync(ICollection<IEventBase> events) => Task.FromResult(SaveEvents(events));

    public int DeleteEvents(ICollection<string> eventIds)
    {
        if (eventIds.IsNullOrEmpty())
            return 0;

        return eventIds.Count(eventId => _events.TryRemove(eventId, out _));
    }

    public Task<int> DeleteEventsAsync(ICollection<string> eventIds) => Task.FromResult(DeleteEvents(eventIds));
}
