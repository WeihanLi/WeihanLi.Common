// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WeihanLi.Common.Helpers.Hosting;

public abstract class CronBasedBackgroundService : BackgroundService
{
    protected abstract string CronExpression { get; }

    protected abstract Task TimedTask(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var next = CronHelper.GetNextOccurrence(CronExpression);
        while (!stoppingToken.IsCancellationRequested && next.HasValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (now >= next)
            {
                _ = TimedTask(stoppingToken);
                next = CronHelper.GetNextOccurrence(CronExpression);
                if (!next.HasValue) break;
            }
            else
            {
                var delay = next.Value - DateTimeOffset.UtcNow;
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}

public abstract class CronBasedBackgroundServiceWithDiagnostic : CronBasedBackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    protected CronBasedBackgroundServiceWithDiagnostic(IServiceProvider serviceProvider)
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
