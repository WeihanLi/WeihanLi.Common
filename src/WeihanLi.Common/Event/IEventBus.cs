namespace WeihanLi.Common.Event
{
    public interface IEventBus
    {
        /// <summary>
        /// add event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Subscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent> where TEvent : IEventBase;

        /// <summary>
        /// remove event handler for event
        /// </summary>
        /// <typeparam name="TEvent">TEvent</typeparam>
        /// <typeparam name="TEventHandler">TEventHandler</typeparam>
        /// <returns>whether the operation success</returns>
        bool Unsubscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent> where TEvent : IEventBase;

        /// <summary>
        /// publish event
        /// </summary>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <param name="event">event</param>
        /// <returns>whether the operation success</returns>
        bool Publish<TEvent>(TEvent @event) where TEvent : IEventBase;
    }
}
