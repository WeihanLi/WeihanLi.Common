// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace WeihanLi.Common.Helpers.Hosting;

public abstract class CronBasedBackgroundService : BackgroundService
{
    protected abstract string CronExpression { get; }

    protected abstract Task ExecuteTaskAsync(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var next = CronHelper.GetNextOccurrence(CronExpression);
        while (!stoppingToken.IsCancellationRequested && next.HasValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (now >= next)
            {
                _ = ExecuteTaskAsync(stoppingToken);
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
    private readonly Counter<int> _executeCounter;

    protected CronBasedBackgroundServiceWithDiagnostic(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _executeCounter = DiagnosticHelper.Meter.CreateCounter<int>("cron-service-executed-counter", "count", "CronBasedBackgroundService execute count(status:[0: success, -1: error])");
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(GetType());
    }

    protected ILogger Logger { get; }

    protected abstract Task ExecuteTaskInternalAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken);

    protected override async Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        using var activity = DiagnosticHelper.ActivitySource.StartActivity();
        try
        {
            Logger.LogInformation("BackgroundService execute begin");
            await ExecuteTaskInternalAsync(scope.ServiceProvider, stoppingToken);
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
