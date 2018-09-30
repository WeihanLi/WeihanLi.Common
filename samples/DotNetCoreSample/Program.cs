using System;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

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

            DependencyResolver.SetDependencyResolver(serviceCollection);
            DependencyInjectionTest.Test();

            //var builder = new ContainerBuilder();
            //builder.RegisterType<MonkeyKing>().As<IFly>();

            //DependencyResolver.SetDependencyResolver((IServiceProvider)new AutofacDependencyResolver(builder.Build()));

            //DependencyInjectionTest.Test();

            //int a = 1;
            //Console.WriteLine(JsonConvert.SerializeObject(a));// output 1

            // log test
            //ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();
            //LogHelper.AddMicrosoftLogging(loggerFactory);
            //var logger = new Logger<Program>(loggerFactory);
            //logger.LogInformation("Logging information from Microsoft.Extensions.Logging");

            //InvokeHelper.TryInvoke(DataExtensionTest.MainTest);

            //TaskTest.TaskWhenAllTest().GetAwaiter().GetResult();

            //Base64UrlEncodeTest.MainTest();

            Console.ReadLine();
        }
    }
}
