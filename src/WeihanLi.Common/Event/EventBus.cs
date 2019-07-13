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
            var hanlders = _eventStore.GetEventHandlerTypes<TEvent>();
            if (hanlders.Count > 0)
            {
                var handlerTasks = new List<Task>();

                foreach (var handlerType in hanlders)
                {
                    try
                    {
                        var handler = DependencyResolver.Current.GetService(handlerType) as IEventHandler<TEvent>;
                        if (handler != null)
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
