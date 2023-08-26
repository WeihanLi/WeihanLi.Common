// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;
using WeihanLi.Common.Helpers.Hosting;

namespace DotNetCoreSample;

public static class AppHostTest
{
    public static async Task MainTest()
    {
        var builder = AppHost.CreateBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Logging.AddJsonConsole(options =>
        {
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
            options.JsonWriterOptions = new JsonWriterOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        });
        builder.AddHostedService<TimerService>();
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
