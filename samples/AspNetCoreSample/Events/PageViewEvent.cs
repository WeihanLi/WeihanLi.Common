using WeihanLi.Common.Event;

namespace AspNetCoreSample.Events;

public class PageViewEvent
{
    public string? Path { get; set; }
}

public class PageViewEventHandler : EventHandlerBase<PageViewEvent>
{
    public static int Count;

    public override Task Handle(PageViewEvent @event, EventProperties eventProperties)
    {
        Interlocked.Increment(ref Count);
        return Task.CompletedTask;
    }
}
