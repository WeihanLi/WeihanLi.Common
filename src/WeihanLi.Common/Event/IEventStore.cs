// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventStore
{
    Task<int> SaveEventsAsync(ICollection<IEvent> events);
    Task<int> DeleteEventsAsync(ICollection<string> eventIds);
}
