using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventHandler<in TEvent> where TEvent : IEventBase
    {
        /// <summary>
        /// Handler event
        /// </summary>
        /// <param name="event">event</param>
        Task Handle(TEvent @event);
    }
}
