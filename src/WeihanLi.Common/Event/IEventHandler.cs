using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventHandler<TEvent> where TEvent : EventBase
    {
        Task Handle(TEvent @event);
    }
}
