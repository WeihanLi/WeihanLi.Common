using WeihanLi.Common.Event;
using Xunit;

namespace WeihanLi.Common.Test.EventsTest
{
    public class AckQueueTest
    {
        private readonly AckQueue _ackQueue = new();

        [Fact]
        public async Task EnqueueAsync_ShouldAddMessageToQueue()
        {
            var testEvent = new TestEvent { Message = "Test Message" };
            await _ackQueue.EnqueueAsync(testEvent);

            var dequeuedEvent = await _ackQueue.DequeueAsync<TestEvent>();
            Assert.NotNull(dequeuedEvent);
            Assert.Equal(testEvent.Message, dequeuedEvent?.Data.Message);
        }

        [Fact]
        public async Task DequeueAsync_ShouldRetrieveMessageWithoutRemoval()
        {
            var testEvent = new TestEvent { Message = "Test Message" };
            await _ackQueue.EnqueueAsync(testEvent);

            var dequeuedEvent1 = await _ackQueue.DequeueAsync<TestEvent>();
            var dequeuedEvent2 = await _ackQueue.DequeueAsync<TestEvent>();

            Assert.NotNull(dequeuedEvent1);
            Assert.Equal(testEvent.Message, dequeuedEvent1?.Data.Message);
            Assert.Null(dequeuedEvent2);
        }

        [Fact]
        public async Task AckMessageAsync_ShouldAcknowledgeAndRemoveMessage()
        {
            var testEvent = new TestEvent { Message = "Test Message" };
            await _ackQueue.EnqueueAsync(testEvent);

            var dequeuedEvent = await _ackQueue.DequeueAsync<TestEvent>();
            Assert.NotNull(dequeuedEvent);

            await _ackQueue.AckMessageAsync(dequeuedEvent!.Properties.EventId);

            var dequeuedEventAfterAck = await _ackQueue.DequeueAsync<TestEvent>();
            Assert.Null(dequeuedEventAfterAck);
        }

        [Fact]
        public async Task RequeueUnAckedMessagesAsync_ShouldRequeueUnAckedMessagesAfterTimeout()
        {
            var testEvent = new TestEvent { Message = "Test Message" };
            await _ackQueue.EnqueueAsync(testEvent);

            var dequeuedEvent = await _ackQueue.DequeueAsync<TestEvent>();
            Assert.NotNull(dequeuedEvent);

            // Simulate timeout
            await Task.Delay(TimeSpan.FromMinutes(2));

            _ackQueue.RequeueUnAckedMessages();

            var requeuedEvent = await _ackQueue.DequeueAsync<TestEvent>();
            Assert.NotNull(requeuedEvent);
            Assert.Equal(testEvent.Message, requeuedEvent?.Data.Message);
        }

        private class TestEvent
        {
            public string Message { get; set; }
        }
    }
}
