using System.Threading.Tasks;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
    /// <summary>
    /// EventBus in process
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private static readonly ILogHelperLogger _logger = Helpers.LogHelper.GetLogger<EventBus>();

        private readonly IEventSubscriptionManager _subscriptionManager;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public EventBus(IEventSubscriptionManager subscriptionManager, IEventHandlerFactory eventHandlerFactory)
        {
            _subscriptionManager = subscriptionManager;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task<bool> PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventBase
        {
            var handlers = _eventHandlerFactory.GetHandlers<TEvent>();
            if (handlers.Count > 0)
            {
                var handlerTasks = new Task[handlers.Count];

                handlers.ForEach((handler, index) =>
                {
                    handlerTasks[index] = handler.Handle(@event).ContinueWith(r =>
                    {
                        if (r.IsFaulted)
                        {
                            _logger.Error(r.Exception?.Unwrap(),
                                $"handle event [{typeof(TEvent).FullName}] error, eventHandlerType:{handler.GetType().FullName}");
                        }
                    });
                });

                await handlerTasks.WhenAll().ConfigureAwait(false);

                return true;
            }
            return false;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _subscriptionManager.Subscribe<TEvent, TEventHandler>();
        }

        public Task<bool> SubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent>
        {
            return _subscriptionManager.SubscribeAsync<TEvent, TEventHandler>();
        }

        public bool UnSubscribe<TEvent, TEventHandler>()
            where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _subscriptionManager.UnSubscribe<TEvent, TEventHandler>();
        }

        public Task<bool> UnSubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _subscriptionManager.UnSubscribeAsync<TEvent, TEventHandler>();
        }
    }
}
