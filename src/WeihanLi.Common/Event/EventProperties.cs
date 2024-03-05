// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public sealed class EventProperties: Dictionary<string, object?>, IEventBase
{
    public DateTimeOffset EventAt { get; init; }
    public string EventId { get; init; }
}
