// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers.Hosting;
#if NET6_0_OR_GREATER
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
#endif
