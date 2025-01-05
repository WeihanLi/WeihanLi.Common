using WeihanLi.Common.Event;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.EventsTest;

public class TestEvent : EventBase
{
    public string Name { get; set; } = null!;
}

public class TestEventHandler : EventHandlerBase<TestEvent>
{
    public static int Count;

    public override Task Handle(TestEvent @event, EventProperties eventProperties)
    {
        Count++;
        return Task.CompletedTask;
    }
}

public class EventBaseTest
{
    [Fact]
    public void EventDeserializeTest()
    {
        var testEvent = new TestEvent()
        {
            Name = "1213"
        };
        var eventDataJson = testEvent.ToJson();
        var deserializedEvent = eventDataJson.JsonToObject<TestEvent>();
        Assert.Equal(testEvent.EventId, deserializedEvent.EventId);
        Assert.Equal(testEvent.EventAt, deserializedEvent.EventAt);
        Assert.Equal(testEvent.Name, deserializedEvent.Name);
    }

    [Fact]
    public void EventMessageExtensionsTest()
    {
        var testEvent = new TestEvent()
        {
            Name = "1213"
        };
        var eventMsg = testEvent.ToEventRawMsg();
        var eventFromMsg = eventMsg.ToEvent<TestEvent>();
        Assert.Equal(typeof(TestEvent), eventFromMsg.GetType());

        var deserializedEvent = eventFromMsg as TestEvent;
        Assert.NotNull(deserializedEvent);
        Assert.Equal(testEvent.EventId, deserializedEvent.EventId);
        Assert.Equal(testEvent.EventAt, deserializedEvent.EventAt);
        Assert.Equal(testEvent.Name, deserializedEvent.Name);
    }
    
    [Fact]
    public void EventMessageExtensions2Test()
    {
        var testEvent = new TestEvent()
        {
            Name = "1213"
        };
        var eventMsg = testEvent.ToEventMsg();
        var eventFromMsg = eventMsg.ToEvent<EventWrapper<TestEvent>>();
        Assert.Equal(typeof(EventWrapper<TestEvent>), eventFromMsg.GetType());

        var deserializedEvent = eventFromMsg.Data;
        Assert.NotNull(deserializedEvent);
        Assert.Equal(testEvent.EventId, deserializedEvent.EventId);
        Assert.Equal(testEvent.EventAt, deserializedEvent.EventAt);
        Assert.Equal(testEvent.Name, deserializedEvent.Name);
        Assert.Equal(testEvent.EventId, eventFromMsg.Properties.EventId);
        Assert.Equal(testEvent.EventAt, eventFromMsg.Properties.EventAt);
    }
}
