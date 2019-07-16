using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventHandler<in TEvent> where TEvent : EventBase
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}
