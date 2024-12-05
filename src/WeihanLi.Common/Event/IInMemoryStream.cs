namespace WeihanLi.Common.Event;

public interface IInMemoryStream
{
    Task AddMessageAsync(string streamName, StreamMessage message);
    Task<ICollection<StreamMessage>> ReadMessagesAsync(string streamName, int count);
    Task AcknowledgeMessageAsync(string streamName, string messageId);
}
