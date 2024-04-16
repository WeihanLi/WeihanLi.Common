using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace DotNetCoreSample;

internal class EventTest
{
    public static async Task MainTest()
    {
        {
            Console.WriteLine(@"EventBus dependency injection sample");
            var services = new ServiceCollection();
            services.AddEvents()
                .RegisterEventHandlers()
                .AddEventHandler(new DelegateEventHandler<CounterEvent>(e => Console.WriteLine(@$"Delegate event handler handling: {e.ToJson()}")))
                ;
            await using var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            await eventBus.PublishAsync(new CounterEvent { Counter = 1 });
            await eventBus.PublishAsync(new CounterEvent { Counter = 2 });
            await eventBus.PublishAsync(new CounterEvent() { Counter = 3 }, new EventProperties()
            {
                TraceId = Guid.NewGuid().ToString()
            });
        }

        {
            // none dependencyInjection sample
            Console.WriteLine(@"EventBus sample without dependency injection");
            var eventBus = new EventBus();
            await eventBus.SubscribeAsync<CounterEvent, CounterEventHandler1>();
            await eventBus.SubscribeAsync<CounterEvent, CounterEventHandler2>();
            var delegateEventHandler = new DelegateEventHandler<CounterEvent>(e =>
                Console.WriteLine(@$"Delegate event handler handling: {e.ToJson()}"));
            await eventBus.SubscribeAsync(delegateEventHandler);

            await eventBus.PublishAsync(new CounterEvent() { Counter = 1 });
            await eventBus.PublishAsync(new CounterEvent() { Counter = 2 });
            
            await eventBus.PublishAsync(new CounterEvent() { Counter = 3 }, new EventProperties()
            {
                TraceId = Guid.NewGuid().ToString()
            });
        }
    }
}

public class CounterEvent
{
    public int Counter { get; set; }
}

internal class CounterEventHandler1 : EventHandlerBase<CounterEvent>
{
    public override Task Handle(CounterEvent @event, EventProperties eventProperties)
    {
        Console.WriteLine($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}, eventProperties: {JsonSerializer.Serialize(eventProperties)}");
        return Task.CompletedTask;
    }
}

internal class CounterEventHandler2 : EventHandlerBase<CounterEvent>
{
    public override Task Handle(CounterEvent @event, EventProperties eventProperties)
    {
        Console.WriteLine($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}, eventProperties: {eventProperties.ToJson()}");
        return Task.CompletedTask;
    }
}
