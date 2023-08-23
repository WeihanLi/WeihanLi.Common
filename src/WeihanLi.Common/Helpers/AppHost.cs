// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.
#if NET6_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WeihanLi.Common.Helpers;

public interface IAppHost
{
    IConfiguration Configuration { get; }
    IServiceProvider Services { get; }
    Task RunAsync(CancellationToken cancellationToken = default);
}

public interface IAppHostBuilder
{
    /// <summary>
    /// Gets the set of key/value configuration properties.
    /// </summary>
    ConfigurationManager Configuration { get; }
    
    /// <summary>
    /// Gets a collection of logging providers for the application to compose. This is useful for adding new logging providers.
    /// </summary>
    ILoggingBuilder Logging { get; }

    /// <summary>
    /// Gets a collection of services for the application to compose. This is useful for adding user provided or framework provided services.
    /// </summary>
    IServiceCollection Services { get; }
}

public sealed class AppHostBuilderSettings
{
    public ConfigurationManager? Configuration { get; set; }
}

public sealed class AppHostBuilder : IAppHostBuilder
{
    private bool _hostBuilt;
    private readonly ServiceCollection _serviceCollection;
    internal AppHostBuilder(AppHostBuilderSettings? settings)
    {
        settings ??= new();
        _serviceCollection = new ServiceCollection();
        Configuration = settings.Configuration ?? new ConfigurationManager();
        
        Logging = new LoggingBuilder(Services);
        
        _serviceCollection.AddSingleton<IConfiguration>(Configuration);
        _serviceCollection.AddLogging();
    }

    public ConfigurationManager Configuration { get; }
    public ILoggingBuilder Logging { get; }
    public IServiceCollection Services => _serviceCollection;

    public IAppHost Build()
    {
        if (_hostBuilt)
        {
            throw new InvalidOperationException("The AppHost had been created");
        }
        _hostBuilt = true;
#if NET7_0_OR_GREATER
        _serviceCollection.MakeReadOnly();
#endif
        var services = Services.BuildServiceProvider();
        return new AppHost(services, Configuration);
    }

    private sealed class LoggingBuilder: ILoggingBuilder
    {
        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }
        public IServiceCollection Services { get; }
    }
}

public sealed class AppHost : IAppHost
{
    private const string
        AppHostStartingMessage = "AppHost starting",
        AppHostStartedMessage = "AppHost started. Press Ctrl+C to shut down",
        AppHostStoppedMessage = "AppHost stopped"
        ;
    
    private readonly ILogger _logger;
    public AppHost(IServiceProvider services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
        _logger = services.GetRequiredService<ILogger<AppHost>>();
    }
    
    public IConfiguration Configuration { get; }
    public IServiceProvider Services { get; }
    
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine(AppHostStartingMessage);
        _logger.LogInformation(AppHostStartingMessage);
        using var hostStopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(InvokeHelper.GetExitToken(), cancellationToken);
        var waitForStopTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        hostStopTokenSource.Token.Register(() => waitForStopTask.TrySetResult());
        Debug.WriteLine(AppHostStartedMessage);
        _logger.LogInformation(AppHostStartedMessage);
        await waitForStopTask.Task.ConfigureAwait(false);
        Debug.WriteLine(AppHostStoppedMessage);
        _logger.LogInformation(AppHostStoppedMessage);
    }
    
    public static AppHostBuilder CreateBuilder(AppHostBuilderSettings? settings = null)
    {
        return new AppHostBuilder(settings);
    }
}

#endif
