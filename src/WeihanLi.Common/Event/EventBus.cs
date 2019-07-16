using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
    /// <summary>
    /// EventBus in process
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IEventStore _eventStore;

        public EventBus(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : EventBase
        {
            var handlers = _eventStore.GetEventHandlerTypes<TEvent>();
            if (handlers.Count > 0)
            {
                var handlerTasks = new List<Task>();

                foreach (var handlerType in handlers)
                {
                    try
                    {
                        if (DependencyResolver.Current.GetService(handlerType) is IEventHandler<TEvent> handler)
                        {
                            handlerTasks.Add(handler.Handle(@event));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        InvokeHelper.OnInvokeException?.Invoke(ex);
                    }
                }
                Task.Run(handlerTasks.WhenAll).ConfigureAwait(false);

                return true;
            }
            return false;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _eventStore.AddSubscription<TEvent, TEventHandler>();
        }

        public bool Unsubscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            return _eventStore.RemoveSubscription<TEvent, TEventHandler>();
        }
    }
}
