// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public static class DelegateEventHandler
{
    public static DelegateEventHandler<TEvent> FromAction<TEvent>(Action<TEvent> action) where TEvent : class => new(action);

    public static DelegateEventHandler<TEvent> FromFunc<TEvent>(Func<TEvent, Task> func) where TEvent : class => new(func);
}

public sealed class DelegateEventHandler<TEvent> : EventHandlerBase<TEvent>
    where TEvent : class
{
    private readonly Func<TEvent, Task> _func;

    public DelegateEventHandler(Action<TEvent> action)
    {
        Guard.NotNull(action);
        _func = action.WrapTask();
    }

    public DelegateEventHandler(Func<TEvent, Task> func)
    {
        _func = Guard.NotNull(func);
    }

    public override Task Handle(TEvent @event, EventProperties eventProperties)
    {
        return _func.Invoke(@event);
    }
}
