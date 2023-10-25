// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;

#if NET6_0_OR_GREATER

namespace WeihanLi.Common.Helpers.Hosting;

public abstract class TimerBaseBackgroundService : BackgroundService
{
    protected abstract TimeSpan Period { get; }
    protected abstract Task ExecuteTaskAsync(CancellationToken cancellationToken);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            await ExecuteTaskAsync(stoppingToken).ConfigureAwait(false);
        }
    }
}

public abstract class TimerBaseBackgroundServiceWithDiagnostic : TimerBaseBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Counter<int> _executeCounter;

    protected TimerBaseBackgroundServiceWithDiagnostic(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _executeCounter = DiagnosticHelper.Meter.CreateCounter<int>("timer-service-executed-counter", "count", "TimerBaseBackgroundService execute count(status:[0: success, -1: error])");
        Logger = _serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(GetType());
    }

    protected ILogger Logger { get; }

    protected abstract Task ExecuteTaskInternalAsync(IServiceProvider serviceProvider, Activity? activity, CancellationToken cancellationToken);

    protected override async Task ExecuteTaskAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        using var activity = DiagnosticHelper.ActivitySource.StartActivity();
        try
        {
            Logger.LogInformation("BackgroundService execute begin");
            await ExecuteTaskInternalAsync(scope.ServiceProvider, activity, cancellationToken);
            Logger.LogInformation("BackgroundService execute end");
            if (_executeCounter.Enabled) _executeCounter.Add(1, new KeyValuePair<string, object?>("status", "0"));
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            Logger.LogError(e, "BackgroundService execute exception");
            if (_executeCounter.Enabled) _executeCounter.Add(1, new KeyValuePair<string, object?>("status", "-1"));
        }
    }
}

#endif
