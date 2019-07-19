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
        private static readonly ILogHelperLogger Logger = Helpers.LogHelper.GetLogger<EventBus>();

        private readonly IEventStore _eventStore;
        private readonly IServiceProvider _serviceProvider;

        public EventBus(IEventStore eventStore, IServiceProvider serviceProvider = null)
        {
            _eventStore = eventStore;
            _serviceProvider = serviceProvider ?? DependencyResolver.Current;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : IEventBase
        {
            if (!_eventStore.HasSubscriptionsForEvent<TEvent>())
            {
                return false;
            }
            var handlers = _eventStore.GetEventHandlerTypes<TEvent>();
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
                        Logger.Error(ex, $"handle event [{_eventStore.GetEventKey<TEvent>()}] error, eventHandlerType:{handlerType.FullName}");
                    }
                }
                handlerTasks.WhenAll().ConfigureAwait(false);

                return true;
            }
            return false;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _eventStore.AddSubscription<TEvent, TEventHandler>();
        }

        public bool Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _eventStore.RemoveSubscription<TEvent, TEventHandler>();
        }
    }
}
