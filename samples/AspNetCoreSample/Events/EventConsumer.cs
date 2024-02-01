using WeihanLi.Common.Event;
using WeihanLi.Extensions;

namespace AspNetCoreSample.Events;

public class EventConsumer
  (IEventQueue eventQueue, IEventHandlerFactory eventHandlerFactory)
  : BackgroundService
{
    private readonly IEventQueue _eventQueue = eventQueue;
    private readonly IEventHandlerFactory _eventHandlerFactory = eventHandlerFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queues = await _eventQueue.GetQueuesAsync();
            if (queues.Count > 0)
            {
                await queues.Select(async q =>
                        {
                            var @event = await _eventQueue.DequeueAsync(q);
                            if (null != @event)
                            {
                                var handlers = _eventHandlerFactory.GetHandlers(@event.GetType());
                                if (handlers.Count > 0)
                                {
                                    await handlers
                                            .Select(h => h.Handle(@event))
                                            .WhenAll()
                                        ;
                                }
                            }
                        })
                        .WhenAll()
                    ;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
