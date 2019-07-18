using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventHandler<in TEvent> where TEvent : IEventBase
    {
        Task Handle(TEvent @event);
    }
}
