using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event;

public sealed class AckQueueOptions
{
    public TimeSpan AckTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public bool AutoRequeue { get; set; } = true;

    public TimeSpan RequeuePeriod { get; set; } = TimeSpan.FromMinutes(1);
}

public sealed class AckQueue : DisposableBase
{
    private readonly AckQueueOptions _options;
    private readonly ConcurrentQueue<IEvent> _queue = new();
    private readonly ConcurrentDictionary<string, IEvent> _unAckedMessages = new();
    private readonly Timer? _timer;

    public AckQueue() : this(new()) { }

    public AckQueue(AckQueueOptions options)
    {
        _options = options;
        if (options.AutoRequeue)
        {
            _timer = new Timer(_ => RequeueUnAckedMessages(), null, options.RequeuePeriod, options.RequeuePeriod);
        }
    }

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
            _unAckedMessages.TryAdd(eventWrapper.Properties.EventId, eventWrapper);
            return Task.FromResult((IEvent<TEvent>?)eventWrapper);
        }

        return Task.FromResult<IEvent<TEvent>?>(null);
    }

    public Task AckMessageAsync(string eventId)
    {
        _unAckedMessages.TryRemove(eventId, out _);
        return Task.CompletedTask;
    }

    public void RequeueUnAckedMessages()
    {
        foreach (var message in _unAckedMessages)
        {
            if (DateTimeOffset.Now - message.Value.Properties.EventAt > _options.AckTimeout)
            {
                if (_unAckedMessages.TryRemove(message.Key, out var eventWrapper)
                    && eventWrapper != null)
                {
                    _queue.Enqueue(eventWrapper);
                }
            }
        }
    }

    public async IAsyncEnumerable<IEvent> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var eventWrapper))
            {
                _unAckedMessages.TryAdd(eventWrapper.Properties.EventId, eventWrapper);
                yield return eventWrapper;
            }

            await Task.Delay(200, cancellationToken);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _timer?.Dispose();
        base.Dispose(disposing);
    }
}
