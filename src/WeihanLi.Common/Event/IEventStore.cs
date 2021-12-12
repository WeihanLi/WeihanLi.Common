namespace WeihanLi.Common.Event;

public interface IEventStore
{
    int SaveEvents(params IEventBase[] events);

    Task<int> SaveEventsAsync(params IEventBase[] events);

    int DeleteEvents(params string[] eventIds);

    Task<int> DeleteEventsAsync(params string[] eventIds);
}
