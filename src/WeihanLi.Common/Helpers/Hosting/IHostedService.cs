// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers.Hosting;

public interface IHostedService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}

public interface IHostedLifecycleService : IHostedService
{
    Task StartingAsync(CancellationToken cancellationToken);
    Task StartedAsync(CancellationToken cancellationToken);
    
    Task StoppingAsync(CancellationToken cancellationToken);
    Task StoppedAsync(CancellationToken cancellationToken);
}
