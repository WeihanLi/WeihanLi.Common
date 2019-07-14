using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Event
{
    public class DelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : EventBase
    {
        private readonly Func<TEvent, Task> _func;

        public DelegateEventHandler(Action<TEvent> action)
        {
            if (null == action) throw new ArgumentNullException(nameof(action));

            _func = TaskHelper.FromAction(action);
        }

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
