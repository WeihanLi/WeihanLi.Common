// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Event;

public interface IEventPublisher
{
    /// <summary>
    /// publish an event async
    /// </summary>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <param name="event">event data</param>
    /// <param name="properties">properties</param>
    /// <returns>whether the operation succeed</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<bool> PublishAsync<TEvent>(TEvent @event, EventProperties? properties = null);
}
