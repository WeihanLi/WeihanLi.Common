// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
        builder.Logging.AddNewtonJsonConsole(options =>
        {
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        });
        // builder.AddHostedService<TimerService>();
        builder.AddHostedService<DiagnosticBackgroundService>();
        var cts = new CancellationTokenSource(5000);
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
