// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public interface IEventStore
{
    int SaveEvents(ICollection<IEventBase> events);
    Task<int> SaveEventsAsync(ICollection<IEventBase> events);
    int DeleteEvents(ICollection<string> eventIds);
    Task<int> DeleteEventsAsync(ICollection<string> eventIds);
}
