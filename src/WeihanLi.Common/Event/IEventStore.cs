using System.Threading.Tasks;

namespace WeihanLi.Common.Event
{
    public interface IEventStore
    {
        int SaveEvents(params IEventBase[] events);

        Task<int> SaveEventsAsync(params IEventBase[] events);

        int DeleteEvents(params string[] events);

        Task<int> DeleteEventsAsync(params string[] events);
    }
}
