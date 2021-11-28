using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetCoreSample
{
    internal class LoggerTest
    {
        public static void MainTest()
        {
            LogHelper.ConfigureLogging(builder =>
            {
                builder
                    .AddConsole()
                    //.AddLog4Net()
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
                .AddLogging(builder => builder.AddConsole())
                .AddSingleton(typeof(GenericTest<>))
                .BuildServiceProvider();
            services.GetRequiredService<GenericTest<int>>()
                .Test();
            services.GetRequiredService<GenericTest<string>>()
                .Test();

            Console.WriteLine();

            services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().UseCustomGenericLogger(options => options.FullNamePredict = _ => true))
                .AddSingleton(typeof(GenericTest<>))
                .BuildServiceProvider();
            services.GetRequiredService<GenericTest<int>>()
                .Test();
            services.GetRequiredService<GenericTest<string>>()
                .Test();
        }

        private class GenericTest<T>
        {
            private readonly ILogger<GenericTest<T>> _logger;

            public GenericTest(ILogger<GenericTest<T>> logger)
            {
                _logger = logger;
            }

            public void Test() => _logger.LogInformation("test");
        }
    }
}
