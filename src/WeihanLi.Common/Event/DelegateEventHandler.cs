// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Event;

public sealed class DelegateEventHandler<TEvent> : EventHandlerBase<TEvent>
{
    private readonly Func<TEvent, EventProperties, Task> _func;

    public DelegateEventHandler(Action<TEvent> action)
    {
        Guard.NotNull(action);
        _func = (e, _) =>
        {
            action(e);
            return Task.CompletedTask;
        };
    }

    public DelegateEventHandler(Action<TEvent, EventProperties> action)
    {
        Guard.NotNull(action);
        _func = (e, properties) =>
        {
            action(e, properties);
            return Task.CompletedTask;
        };
    }

    public DelegateEventHandler(Func<TEvent, Task> func)
    {
        Guard.NotNull(func);
        _func = (e, _) => func(e);
    }

    public DelegateEventHandler(Func<TEvent, EventProperties, Task> func)
    {
        _func = Guard.NotNull(func);
    }

    public override Task Handle(TEvent @event, EventProperties properties)
    {
        return _func.Invoke(@event, properties);
    }
}
