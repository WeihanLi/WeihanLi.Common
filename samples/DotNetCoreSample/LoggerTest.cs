﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetCoreSample;

internal class LoggerTest
{
    public static void MainTest()
    {
        LogHelper.ConfigureLogging(builder =>
        {
            builder
                .AddConsole()
                //.AddSerilog(loggerConfig => loggerConfig.WriteTo.Console())
                //.WithMinimumLevel(LogHelperLogLevel.Info)
                //.WithFilter((category, level) => level > LogHelperLogLevel.Error && category.StartsWith("System"))
                //.EnrichWithProperty("Entry0", ApplicationHelper.ApplicationName)
                //.EnrichWithProperty("Entry1", ApplicationHelper.ApplicationName, e => e.LogLevel >= LogHelperLogLevel.Error)
                ;
        });

        var abc = "1233";
        var logger = LogHelper.GetLogger<LoggerTest>();
        logger.Debug("12333 {abc}", abc);
        logger.Trace("122334334");
        logger.Info($"122334334 {abc}");

        logger.Warn("12333, err:{err}", "hahaha");
        logger.Error("122334334");
        logger.Fatal("12333");
    }

    public static void MicrosoftLoggingTest()
    {
        var services = new ServiceCollection()
            .AddLogging(builder =>
                // builder.AddConsole()
                builder.AddFile(options => options.LogFormatter = (category, level, exception, msg, timestamp) => 
                    $"{timestamp} - [{category}] {level} - {msg}\n{exception}")
                )
            .AddSingleton(typeof(GenericTest<>))
            .BuildServiceProvider();

        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("test");
        while (!ApplicationHelper.ExitToken.IsCancellationRequested)
        {
            logger.LogInformation("Echo time: {Time}", DateTimeOffset.Now);
            Thread.Sleep(500);
        }

        ConsoleHelper.ReadKeyWithPrompt();
        services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("test")
            .LogInformation("test 123");
        services.GetRequiredService<GenericTest<int>>()
            .Test();
        services.GetRequiredService<GenericTest<string>>()
            .Test();

        Console.WriteLine();

        services = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole().UseCustomGenericLogger())
            .AddSingleton(typeof(GenericTest<>))
            .BuildServiceProvider();
        services.GetRequiredService<GenericTest<int>>()
            .Test();
        services.GetRequiredService<GenericTest<string>>()
            .Test();
    }

    private class GenericTest<T>(ILogger<GenericTest<T>> logger)
    {
        public void Test() => logger.LogInformation("test");
    }
}
