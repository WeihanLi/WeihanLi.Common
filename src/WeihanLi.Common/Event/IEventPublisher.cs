using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventPublisher
    {
        /// <summary>
        /// publish an event async
        /// </summary>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <param name="event">event data</param>
        /// <returns>whether the operation succeed</returns>
        bool Publish<TEvent>(TEvent @event) where TEvent : class, IEventBase;

        /// <summary>
        /// publish an event async
        /// </summary>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <param name="event">event data</param>
        /// <returns>whether the operation succeed</returns>
        Task<bool> PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventBase;
    }
}
