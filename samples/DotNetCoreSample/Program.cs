using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.LogInit();
            Console.WriteLine(SystemHelper.OsType);
            // ReSharper disable once LocalizableElement
            Console.WriteLine("----------DotNetCoreSample----------");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IFly, MonkeyKing>();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            serviceCollection.AddSingleton(configuration);

            DependencyResolver.SetDependencyResolver(serviceCollection);

            //DependencyInjectionTest.Test();

            //var builder = new ContainerBuilder();
            //builder.RegisterType<MonkeyKing>().As<IFly>();

            //DependencyResolver.SetDependencyResolver((IServiceProvider)new AutofacDependencyResolver(builder.Build()));

            //DependencyInjectionTest.Test();

            //int a = 1;
            //Console.WriteLine(JsonConvert.SerializeObject(a));// output 1

            // log test
            // LoggerTest.MainTest();
            //ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();
            //LogHelper.AddMicrosoftLogging(loggerFactory);
            //var logger = new Logger<Program>(loggerFactory);
            //logger.LogInformation("Logging information from Microsoft.Extensions.Logging");

            InvokeHelper.TryInvoke(DataExtensionTest.MainTest);

            //TaskTest.TaskWhenAllTest().GetAwaiter().GetResult();

            //Base64UrlEncodeTest.MainTest();

            var a = new { Name = "2123" };
            var name = a.GetProperty("Name").GetValueGetter().Invoke(a);
            Console.WriteLine(name);
            Console.ReadLine();
        }
    }
}
