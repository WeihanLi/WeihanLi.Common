namespace WeihanLi.Common.Event
{
    public interface IEventBus
    {
        /// <summary>
        /// register event handler
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Subscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent> where TEvent : EventBase;

        /// <summary>
        /// unregister event handler
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Unsubscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent> where TEvent : EventBase;

        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <param name="event">event</param>
        /// <returns>whether the operation success</returns>
        bool Publish<TEvent>(TEvent @event) where TEvent : EventBase;
    }
}
