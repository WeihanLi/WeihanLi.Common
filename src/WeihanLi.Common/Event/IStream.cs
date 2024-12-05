namespace WeihanLi.Common.Event;

public interface IStream
{
    Task AddMessageAsync(string streamName, StreamMessage message);
    Task AcknowledgeMessageAsync(string streamName, string messageId);
    Task<int> CountAsync(string streamName, string? start = null, string? end = null);
    Task TrimAsync(string streamName, int maxSize);
    Task TrimAsync(string streamName, TimeSpan maxAge);
    IAsyncEnumerable<StreamMessage> ReadMessagesAsync(string streamName, int count, string? start = null, string? end = null, CancellationToken cancellationToken = default);
}
