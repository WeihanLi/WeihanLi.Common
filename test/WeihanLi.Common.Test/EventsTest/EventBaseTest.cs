using WeihanLi.Common.Event;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.EventsTest
{
    internal class TestEvent : EventBase
    {
        public string Name { get; set; }
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

            var eventMsg = testEvent.ToEventMsg();
            var eventFromMsg = eventMsg.ToEvent();
            Assert.Equal(typeof(TestEvent), eventFromMsg.GetType());

            deserializedEvent = eventFromMsg as TestEvent;
            Assert.NotNull(deserializedEvent);
            Assert.Equal(testEvent.EventId, deserializedEvent.EventId);
            Assert.Equal(testEvent.EventAt, deserializedEvent.EventAt);
            Assert.Equal(testEvent.Name, deserializedEvent.Name);
        }
    }
}
