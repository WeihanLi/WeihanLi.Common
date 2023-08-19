// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;
using WeihanLi.Common.Helpers;

namespace DotNetCoreSample;

public class AppHostTest
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
        var cts = new CancellationTokenSource(5000);
        var app = builder.Build();
        await app.RunAsync(cts.Token);
    }
}
