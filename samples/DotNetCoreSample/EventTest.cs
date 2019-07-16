using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    internal class EventTest
    {
        public static void MainTest()
        {
            var eventBus = DependencyResolver.Current.ResolveService<IEventBus>();
            eventBus.Subscribe<CounterEvent, CounterEventHandler1>();
            eventBus.Subscribe<CounterEvent, CounterEventHandler1>();

            eventBus.Subscribe<CounterEvent, CounterEventHandler2>();

            eventBus.Publish(new CounterEvent { Counter = 1 });
            eventBus.Publish(new CounterEvent { Counter = 2 });
        }
    }

    internal class CounterEvent : EventBase
    {
        public int Counter { get; set; }
    }

    internal class CounterEventHandler1 : IEventHandler<CounterEvent>
    {
        public Task Handle(CounterEvent @event, CancellationToken cancellationToken = default)
        {
            LogHelper.GetLogger<CounterEventHandler1>().Info($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}");
            return Task.CompletedTask;
        }
    }

    internal class CounterEventHandler2 : IEventHandler<CounterEvent>
    {
        public Task Handle(CounterEvent @event, CancellationToken cancellationToken = default)
        {
            LogHelper.GetLogger<CounterEventHandler2>().Info($"Event Info: {@event.ToJson()}, Handler Type:{GetType().FullName}");
            return Task.CompletedTask;
        }
    }
}
