// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WeihanLi.Extensions.Hosting;

public static class HostingExtensions
{
    public static IHostApplicationBuilder ConfigureHostOptions(this IHostApplicationBuilder hostApplicationBuilder, Action<HostOptions> optionsConfigure)
    {
        ArgumentNullException.ThrowIfNull(hostApplicationBuilder);
        ArgumentNullException.ThrowIfNull(optionsConfigure);

        hostApplicationBuilder.Services.Configure(optionsConfigure);
        
        return hostApplicationBuilder;
    }
}
