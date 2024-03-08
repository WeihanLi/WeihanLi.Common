// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Event;

public sealed class EventProperties: Dictionary<string, object?>, IEventBase
{
    public DateTimeOffset EventAt { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string? TraceId { get; set; }
    public string? EventSource { get; set; }
}
