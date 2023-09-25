// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Helpers.Hosting;

namespace DotNetCoreSample;

public static class AppHostTest
{
    public static async Task MainTest()
    {
        var builder = AppHost.CreateBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Logging.AddRelaxedJsonConsole(options => { options.TimestampFormat = "yyyy-MM-dd HH:mm:ss"; });
        // builder.AddHostedService<TimerService>();
        // builder.AddHostedService<DiagnosticBackgroundService>();
        builder.Services.AddSingleton<IWebServer, HttpListenerWebServer>();
        builder.AddHostedService<WebServerHostedService>();
        var cts = new CancellationTokenSource(60_000);
        var app = builder.Build();
        await app.RunAsync(cts.Token);
    }
}

file sealed class TimerService : TimerBaseBackgroundService
{
    protected override TimeSpan Period => TimeSpan.FromSeconds(1);

    protected override Task TimedTask(CancellationToken cancellationToken)
    {
        Console.WriteLine(DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}

file sealed class DiagnosticBackgroundService : CronBasedBackgroundServiceWithDiagnostic
{
    public DiagnosticBackgroundService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override string CronExpression => CronHelper.Secondly;

    protected override Task TimedTask(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Console.WriteLine(DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}

public static partial class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddRelaxedJsonConsole(this ILoggingBuilder loggingBuilder,
        Action<JsonConsoleFormatterOptions>? optionsConfigure = null)
    {
        return loggingBuilder.AddJsonConsole(options =>
        {
            options.JsonWriterOptions = new JsonWriterOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            optionsConfigure?.Invoke(options);
        });
    }
}

file interface IWebServer
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}

file sealed class HttpListenerWebServer : IWebServer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpListener _listener = new();

    public HttpListenerWebServer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.Prefixes.Add("http://localhost:5100/");
        _listener.Start();
        var logger = _serviceProvider.GetRequiredService<ILogger<HttpListenerWebServer>>();
        logger.LogInformation("WebServer started");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var listenerContext = await _listener.GetContextAsync();
            try
            {
                await listenerContext.Response.OutputStream.WriteAsync("Hello World"u8.ToArray(), cancellationToken);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            finally
            {
                listenerContext.Response.Close();
            }
        }

        _listener.Stop();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Stop();
        return Task.CompletedTask;
    }
}

file sealed class WebServerHostedService : BackgroundService
{
    private readonly IWebServer _server;

    public WebServerHostedService(IWebServer server)
    {
        _server = server;
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _server.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _server.StartAsync(stoppingToken);
    }
}
