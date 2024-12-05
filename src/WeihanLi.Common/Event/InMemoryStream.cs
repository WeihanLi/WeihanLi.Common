using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public class InMemoryStream : IStream
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<StreamMessage>> _streams = new();

    public Task AddMessageAsync(string streamName, StreamMessage message)
    {
        var stream = _streams.GetOrAdd(streamName, _ => new ConcurrentQueue<StreamMessage>());
        stream.Enqueue(message);
        return Task.CompletedTask;
    }

    public Task AcknowledgeMessageAsync(string streamName, string messageId)
    {
        if (_streams.TryGetValue(streamName, out var stream))
        {
            var messages = stream.ToArray();
            stream.Clear();
            foreach (var message in messages)
            {
                if (message.Id != messageId)
                {
                    stream.Enqueue(message);
                }
            }
        }
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(string streamName, string? start = null, string? end = null)
    {
        if (_streams.TryGetValue(streamName, out var stream))
        {
            return Task.FromResult(stream.Count(message =>
                (start == null || string.Compare(message.Id, start) >= 0) &&
                (end == null || string.Compare(message.Id, end) <= 0)));
        }
        return Task.FromResult(0);
    }

    public Task TrimAsync(string streamName, int maxSize)
    {
        if (_streams.TryGetValue(streamName, out var stream))
        {
            while (stream.Count > maxSize && stream.TryDequeue(out _))
            {
                // Remove messages until the stream size is within the limit
            }
        }
        return Task.CompletedTask;
    }

    public Task TrimAsync(string streamName, TimeSpan maxAge)
    {
        if (_streams.TryGetValue(streamName, out var stream))
        {
            var cutoffTime = DateTimeOffset.Now - maxAge;
            var messages = stream.ToArray();
            stream.Clear();
            foreach (var message in messages)
            {
                if (message.Timestamp >= cutoffTime)
                {
                    stream.Enqueue(message);
                }
            }
        }
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<StreamMessage> ReadMessagesAsync(string streamName, int count, string? start = null, string? end = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_streams.TryGetValue(streamName, out var stream))
            {
                while (stream.TryDequeue(out var message))
                {
                    yield return message;
                }
            }
            await Task.Delay(100, cancellationToken);
        }
    }
}

public class StreamMessage
{
    public string Id { get; set; }
    public string Data { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Dictionary<string, object?> Properties { get; set; } = new();
}
