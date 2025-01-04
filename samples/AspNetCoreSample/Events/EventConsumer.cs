using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;

namespace AspNetCoreSample.Events;

public class EventConsumer
  (IEventQueue eventQueue, IEventHandlerFactory eventHandlerFactory)
  : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queues = await eventQueue.GetQueuesAsync();
            if (queues.Count > 0)
            {
                await queues.Select(async q =>
                        {
                            await foreach (var e in eventQueue.ReadAll(q, stoppingToken))
                            {
                                var @event = e.Data;
                                Guard.NotNull(@event);
                                var handlers = eventHandlerFactory.GetHandlers(@event.GetType());
                                if (handlers.Count > 0)
                                {
                                    await handlers
                                            .Select(h => h.Handle(@event, e.Properties))
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
