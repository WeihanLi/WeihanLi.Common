﻿using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace DotNetCoreSample;

internal class EventTest
{
    public static async Task MainTest()
    {
        var eventBus = DependencyResolver.ResolveRequiredService<IEventBus>();

        eventBus.Subscribe<CounterEvent, CounterEventHandler1>();
        eventBus.Subscribe<CounterEvent, CounterEventHandler1>();

        eventBus.Subscribe<CounterEvent, CounterEventHandler2>();
        eventBus.Subscribe<CounterEvent, DelegateEventHandler<CounterEvent>>(); // could be used for eventLogging

        await eventBus.PublishAsync(new CounterEvent { Counter = 1 });

        eventBus.UnSubscribe<CounterEvent, CounterEventHandler1>();
        // eventBus.Unsubscribe<CounterEvent, DelegateEventHandler<CounterEvent>>();
        await eventBus.PublishAsync(new CounterEvent { Counter = 2 });
    }
}

public class CounterEvent : EventBase
{
    public int Counter { get; set; }
}

internal class CounterEventHandler1 : EventHandlerBase<CounterEvent>
{
    public override Task Handle(CounterEvent @event, EventProperties eventProperties)
    {
        LogHelper.GetLogger<CounterEventHandler1>().Info($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}");
        return Task.CompletedTask;
    }
}

internal class CounterEventHandler2 : EventHandlerBase<CounterEvent>
{
    public override Task Handle(CounterEvent @event, EventProperties eventProperties)
    {
        LogHelper.GetLogger<CounterEventHandler2>().Info($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}");
        return Task.CompletedTask;
    }
}
