using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace WeihanLi.Common.Event;

public sealed class AckQueue
{
    private readonly ConcurrentQueue<IEvent> _queue = new();
    private readonly ConcurrentDictionary<string, IEvent> _unackedMessages = new();
    private readonly TimeSpan _ackTimeout = TimeSpan.FromMinutes(1);

    public Task EnqueueAsync<TEvent>(TEvent @event, EventProperties? properties = null)
    {
        properties ??= new EventProperties();
        if (string.IsNullOrEmpty(properties.EventId))
        {
            properties.EventId = Guid.NewGuid().ToString();
        }
        if (properties.EventAt == default)
        {
            properties.EventAt = DateTimeOffset.Now;
        }

        var internalEvent = new EventWrapper<TEvent>
        {
            Data = @event,
            Properties = properties
        };

        _queue.Enqueue(internalEvent);
        return Task.CompletedTask;
    }

    public Task<IEvent<TEvent>?> DequeueAsync<TEvent>()
    {
        if (_queue.TryDequeue(out var eventWrapper))
        {
            _unackedMessages.TryAdd(eventWrapper.Properties.EventId, eventWrapper);
            return Task.FromResult((IEvent<TEvent>?)eventWrapper);
        }

        return Task.FromResult<IEvent<TEvent>?>(null);
    }

    public Task AckMessageAsync(string eventId)
    {
        _unackedMessages.TryRemove(eventId, out _);
        return Task.CompletedTask;
    }

    public async Task RequeueUnackedMessagesAsync()
    {
        foreach (var unackedMessage in _unackedMessages)
        {
            if (DateTimeOffset.Now - unackedMessage.Value.Properties.EventAt > _ackTimeout)
            {
                _unackedMessages.TryRemove(unackedMessage.Key, out var eventWrapper);
                if (eventWrapper != null)
                {
                    _queue.Enqueue(eventWrapper);
                }
            }
        }

        await Task.CompletedTask;
    }

    public async IAsyncEnumerable<IEvent> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var eventWrapper))
            {
                _unackedMessages.TryAdd(eventWrapper.Properties.EventId, eventWrapper);
                yield return eventWrapper;
            }

            await Task.Delay(200, cancellationToken);
        }
    }
}
