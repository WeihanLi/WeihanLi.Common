// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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

    protected TimerBaseBackgroundServiceWithDiagnostic(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(GetType());
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
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            Logger.LogError(e, "BackgroundService execute exception");
        }
    }
}

#endif
