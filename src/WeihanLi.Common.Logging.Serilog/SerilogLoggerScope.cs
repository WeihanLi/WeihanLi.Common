﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Serilog.Core;
using Serilog.Events;

// ReSharper disable once CheckNamespace
namespace Serilog.Extensions.Logging;

internal sealed class SerilogLoggerScope : IDisposable
{
    private const string NoName = "None";

    private readonly SerilogLoggerProvider _provider;
    private readonly object? _state;
    private readonly IDisposable? _chainedDisposable;

    // An optimization only, no problem if there are data races on this.
    private bool _disposed;

    public SerilogLoggerScope(SerilogLoggerProvider provider, object? state, IDisposable? chainedDisposable = null)
    {
        _provider = provider;
        _state = state;

        Parent = _provider.CurrentScope;
        _provider.CurrentScope = this;
        _chainedDisposable = chainedDisposable;
    }

    public SerilogLoggerScope? Parent { get; }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            // In case one of the parent scopes has been disposed out-of-order, don't
            // just blindly reinstate our own parent.
            for (var scan = _provider.CurrentScope; scan != null; scan = scan.Parent)
            {
                if (ReferenceEquals(scan, this))
                    _provider.CurrentScope = Parent;
            }

            _chainedDisposable?.Dispose();
        }
    }

    public void EnrichAndCreateScopeItem(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, out LogEventPropertyValue? scopeItem)
    {
        if (_state == null)
        {
            scopeItem = null;
            return;
        }

        if (_state is IEnumerable<KeyValuePair<string, object>> stateProperties)
        {
            scopeItem = null; // Unless it's `FormattedLogValues`, these are treated as property bags rather than scope items.

            foreach (var stateProperty in stateProperties)
            {
                if (stateProperty.Key == SerilogLoggerProvider.OriginalFormatPropertyName && stateProperty.Value is string)
                {
                    scopeItem = new ScalarValue(_state.ToString());
                    continue;
                }

                var key = stateProperty.Key;
                var destructureObject = false;

                if (key.StartsWith("@"))
                {
                    key = key[1..];
                    destructureObject = true;
                }

                var property = propertyFactory.CreateProperty(key, stateProperty.Value, destructureObject);
                logEvent.AddPropertyIfAbsent(property);
            }
        }
        else
        {
            scopeItem = propertyFactory.CreateProperty(NoName, _state).Value;
        }
    }
}
