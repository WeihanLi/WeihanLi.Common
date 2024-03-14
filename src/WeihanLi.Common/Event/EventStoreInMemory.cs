// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public sealed class EventStoreInMemory : IEventStore
{
    private readonly ConcurrentDictionary<string, IEvent> _events = new();

    public Task<int> SaveEventsAsync(ICollection<IEvent> events)
    {
        if (events.IsNullOrEmpty())
            return Task.FromResult(0);

        return Task.FromResult(events.Count(@event => _events.TryAdd(@event.Properties.EventId, @event)));
    }

    private int DeleteEvents(ICollection<string> eventIds)
    {
        return eventIds.IsNullOrEmpty() ? 0 : eventIds.Count(eventId => _events.TryRemove(eventId, out _));
    }

    public Task<int> DeleteEventsAsync(ICollection<string> eventIds) => Task.FromResult(DeleteEvents(eventIds));
}
