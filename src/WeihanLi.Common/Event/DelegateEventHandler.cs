using System;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Event
{
    public class DelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : EventBase
    {
        private readonly Func<TEvent, CancellationToken, Task> _func;

        public DelegateEventHandler(Action<TEvent> action)
        {
            if (null == action) throw new ArgumentNullException(nameof(action));

            _func = action.WrapTask().WrapCancellation();
        }

        public DelegateEventHandler(Func<TEvent, Task> func)
        {
            _func = func.WrapCancellation() ?? throw new ArgumentNullException(nameof(func));
        }

        public DelegateEventHandler(Func<TEvent, CancellationToken, Task> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public Task Handle(TEvent @event, CancellationToken cancellationToken = default)
        {
            return _func.Invoke(@event, cancellationToken);
        }
    }
}
