using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public class DelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : EventBase
    {
        private readonly Func<TEvent, Task> _func;

        public DelegateEventHandler(Func<TEvent, Task> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public Task Handle(TEvent @event)
        {
            return _func.Invoke(@event);
        }
    }
}
