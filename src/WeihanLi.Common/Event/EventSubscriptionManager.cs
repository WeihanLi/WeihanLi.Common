using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventSubscriptionManager
    {
        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Subscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        Task<bool> SubscribeAsync<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Unsubscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        Task<bool> UnsubscribeAsync<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : class, IEventBase;
    }
}
