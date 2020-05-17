using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Event;

namespace AspNetCoreSample.Events
{
    public class PageViewEvent : EventBase
    {
    }

    public class PageViewEventHandler : EventHandlerBase<PageViewEvent>
    {
        public static int Count;

        public override Task Handle(PageViewEvent @event)
        {
            Interlocked.Increment(ref Count);
            return Task.CompletedTask;
        }
    }
}
