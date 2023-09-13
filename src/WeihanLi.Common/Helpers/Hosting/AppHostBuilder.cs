// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WeihanLi.Common.Helpers.Hosting;

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

    public AppHost Build()
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

