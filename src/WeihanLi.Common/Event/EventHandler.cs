using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventHandler
    {
        Task Handle(object eventData);
    }

    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class, IEventBase
    {
        /// <summary>
        /// Handler event
        /// </summary>
        /// <param name="event">event</param>
        Task Handle(TEvent @event);
    }

    public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : class, IEventBase
    {
        public abstract Task Handle(TEvent @event);

        public virtual Task Handle(object eventData) => Handle(eventData as TEvent);
    }
}
