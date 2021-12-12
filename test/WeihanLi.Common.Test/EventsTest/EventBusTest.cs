using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.Event;
using Xunit;

namespace WeihanLi.Common.Test.EventsTest;

public class EventBusTest
{
    private static int _counter, _counter1;
    private readonly IServiceProvider _serviceProvider;

    public EventBusTest()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddEvents()
            .AddEventHandler<TestEvent, TestEventHandler1>()
            .AddEventHandler<TestEvent, TestEventHandler2>()
            .AddEventHandler<TestEvent, TestEventHandler3<TestEvent>>()
            .AddEventHandler<TestEvent1, TestEventHandler3<TestEvent1>>()
            ;
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task MainTest()
    {
        var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(new TestEvent());
        Assert.Equal(3, _counter);
        await eventBus.PublishAsync(new TestEvent1());
        Assert.Equal(1, _counter1);
        await eventBus.PublishAsync(new TestEvent1());
        Assert.Equal(2, _counter1);
    }

    public class TestEvent : EventBase
    {
        public string? Name { get; set; }
    }

    public class TestEvent1 : EventBase
    {
        public string? Name { get; set; }
    }

    public class TestEventHandler1 : EventHandlerBase<TestEvent>
    {
        public override Task Handle(TestEvent @event)
        {
            Interlocked.Increment(ref _counter);
            return Task.CompletedTask;
        }
    }

    public class TestEventHandler2 : EventHandlerBase<TestEvent>
    {
        public override Task Handle(TestEvent @event)
        {
            Interlocked.Increment(ref _counter);
            return Task.CompletedTask;
        }
    }

    public class TestEventHandler3<TEvent> : EventHandlerBase<TEvent>
        where TEvent : class, IEventBase
    {
        public override Task Handle(TEvent @event)
        {
            if (@event.GetType() == typeof(TestEvent))
            {
                Interlocked.Increment(ref _counter);
            }
            else
            {
                Interlocked.Increment(ref _counter1);
            }
            return Task.CompletedTask;
        }
    }
}
