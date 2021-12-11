using System.Threading.Tasks;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.EventsTest
{
    public class TestEvent : EventBase
    {
        public string Name { get; set; } = null!;
    }

    public class TestEventHandler : EventHandlerBase<TestEvent>
    {
        public static int Count;

        public override Task Handle(TestEvent @event)
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
            var eventMsg = testEvent.ToEventMsg();
            var eventFromMsg = eventMsg.ToEvent();
            Assert.Equal(typeof(TestEvent), eventFromMsg.GetType());

            var deserializedEvent = eventFromMsg as TestEvent;
            Assert.NotNull(deserializedEvent);
            Assert.Equal(testEvent.EventId, deserializedEvent!.EventId);
            Assert.Equal(testEvent.EventAt, deserializedEvent.EventAt);
            Assert.Equal(testEvent.Name, deserializedEvent.Name);
        }
    }
}
