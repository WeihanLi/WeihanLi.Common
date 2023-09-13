// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WeihanLi.Common.Helpers.Hosting;

public static class AppHostBuilderExtensions
{
    public static IAppHostBuilder AddHostedService<TService>(this IAppHostBuilder appHostBuilder) 
        where TService : class, IHostedService
    {
        Guard.NotNull(appHostBuilder);
        appHostBuilder.Services.TryAddEnumerable(
            ServiceDescriptor.Describe(typeof(IHostedService), typeof(TService), ServiceLifetime.Singleton)
            );
        return appHostBuilder;
    }

    public static IAppHostBuilder ConfigureHostOptions(this IAppHostBuilder appHostBuilder, Action<AppHostOptions> configure)
    {
        Guard.NotNull(appHostBuilder);
        Guard.NotNull(configure);
        appHostBuilder.Services.Configure(configure);
        return appHostBuilder;
    }
    
    public static IAppHostBuilder ConfigureHostOptions(this IAppHostBuilder appHostBuilder, Action<AppHostOptions, IConfiguration> configure)
    {
        Guard.NotNull(appHostBuilder);
        Guard.NotNull(configure);
        appHostBuilder.Services.Configure<AppHostOptions>(options => configure(options, appHostBuilder.Configuration));
        return appHostBuilder;
    }
}
