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
    protected abstract Task TimedTask(CancellationToken cancellationToken);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            await TimedTask(stoppingToken).ConfigureAwait(false);
        }
    }
}

public abstract class TimerBaseBackgroundServiceWithDiagnostic : TimerBaseBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Counter<int> _executeSuccessCounter, _executeErrorCounter;

    protected TimerBaseBackgroundServiceWithDiagnostic(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(GetType());
        _executeSuccessCounter = DiagnosticHelper.Meter.CreateCounter<int>("timer-service-executed-counter", "count", "Background service count");
        _executeErrorCounter = DiagnosticHelper.Meter.CreateCounter<int>("timer-service-execute-error-counter", "count", "Background service count");
    }
    
    protected ILogger Logger { get;}
    protected abstract Task TimedTask(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    protected override async Task TimedTask(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        using var activity = DiagnosticHelper.ActivitySource.StartActivity();
        try
        {
            Logger.LogInformation("BackgroundService execute begin");
            await TimedTask(scope.ServiceProvider, cancellationToken);
            Logger.LogInformation("BackgroundService execute end");
            if (_executeSuccessCounter.Enabled) _executeSuccessCounter.Add(1);
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            Logger.LogError(e, "BackgroundService execute exception");
            if (_executeErrorCounter.Enabled) _executeErrorCounter.Add(1);
        }
    }
}

#endif
