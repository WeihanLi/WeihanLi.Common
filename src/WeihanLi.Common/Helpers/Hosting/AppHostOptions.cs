// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers.Hosting;

public sealed class AppHostOptions
{
    /// <summary>
    /// The default timeout for <see cref="IHostedService.StopAsync(CancellationToken)"/>.
    /// </summary>
    /// <remarks>
    /// This timeout also encompasses all host services implementing
    /// <see cref="IHostedLifecycleService.StoppingAsync(CancellationToken)"/> and
    /// <see cref="IHostedLifecycleService.StoppedAsync(CancellationToken)"/>.
    /// </remarks>
    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// The default timeout for <see cref="IAppHost.RunAsync(CancellationToken)"/>.
    /// </summary>
    /// <remarks>
    /// This timeout also encompasses all host services implementing
    /// <see cref="IHostedLifecycleService.StartingAsync(CancellationToken)"/> and
    /// <see cref="IHostedLifecycleService.StartedAsync(CancellationToken)"/>.
    /// </remarks>
    public TimeSpan StartupTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Determines if the <see cref="IAppHost"/> will start registered instances of <see cref="IHostedService"/> concurrently or sequentially. Defaults to false.
    /// </summary>
    public bool ServicesStartConcurrently { get; set; }

    /// <summary>
    /// Determines if the <see cref="IAppHost"/> will stop registered instances of <see cref="IHostedService"/> concurrently or sequentially. Defaults to false.
    /// </summary>
    public bool ServicesStopConcurrently { get; set; }
}
