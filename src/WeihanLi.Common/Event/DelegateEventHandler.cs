using System;
using System.Threading.Tasks;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
    public static class DelegateEventHandler
    {
        public static DelegateEventHandler<TEvent> FromAction<TEvent>(Action<TEvent> action) where TEvent : EventBase
        {
            return new DelegateEventHandler<TEvent>(action);
        }

        public static DelegateEventHandler<TEvent> FromFunc<TEvent>(Func<TEvent, Task> func) where TEvent : EventBase
        {
            return new DelegateEventHandler<TEvent>(func);
        }
    }

    public class DelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : EventBase
    {
        private readonly Func<TEvent, Task> _func;

        public DelegateEventHandler(Action<TEvent> action)
        {
            if (null == action) throw new ArgumentNullException(nameof(action));

            _func = action.WrapTask();
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
