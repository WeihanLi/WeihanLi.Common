using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
    /// <summary>
    /// EventBus in process
    /// </summary>
    public class EventBus : IEventBus
    {
        private static readonly ILogHelperLogger _logger = Helpers.LogHelper.GetLogger<EventBus>();

        private readonly IServiceProvider _serviceProvider;
        private readonly IEventSubscriptionManager _subscriptionManager;

        public EventBus(IEventSubscriptionManager subscriptionManager, IServiceProvider serviceProvider = null)
        {
            _subscriptionManager = subscriptionManager;
            _serviceProvider = serviceProvider ?? DependencyResolver.Current;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : class, IEventBase
        {
            var handlers = _subscriptionManager.GetEventHandlerTypes<TEvent>();
            if (handlers.Count > 0)
            {
                var handlerTasks = new List<Task>();
                foreach (var handlerType in handlers)
                {
                    try
                    {
                        if (_serviceProvider.GetServiceOrCreateInstance(handlerType) is IEventHandler<TEvent> handler)
                        {
                            handlerTasks.Add(handler.Handle(@event));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"handle event [{typeof(TEvent).FullName}] error, eventHandlerType:{handlerType.FullName}");
                    }
                }
                handlerTasks.WhenAll().ConfigureAwait(false);

                return true;
            }
            return false;
        }

        public Task<bool> PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventBase
        {
            return Task.FromResult(Publish(@event));
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

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase => _subscriptionManager.GetEventHandlerTypes<TEvent>();
    }
}
