using System;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Common.Logging.Log4Net;
using WeihanLi.Extensions;

// ReSharper disable once LocalizableElement
namespace DotNetCoreSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.AddLogProvider(new Log4NetLogHelperProvider());
            var dataLogger = LogHelper.GetLogger(typeof(DataExtension));
            DataExtension.CommandLogAction = msg => dataLogger.Debug(msg);

            //Console.WriteLine("----------DotNetCoreSample----------");

            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddScoped<IFly, MonkeyKing>();
            //IConfiguration configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //var city = configuration.GetAppSetting("City");
            //var number = configuration.GetAppSetting<int>("Number");
            //System.Console.WriteLine($"City:{city}, Number:{number}");

            //serviceCollection.AddSingleton(configuration);

            //DependencyResolver.SetDependencyResolver(serviceCollection);

            //DependencyInjectionTest.Test();

            //var builder = new ContainerBuilder();
            //builder.RegisterType<MonkeyKing>().As<IFly>();

            //DependencyResolver.SetDependencyResolver((IServiceProvider)new AutofacDependencyResolver(builder.Build()));

            //DependencyInjectionTest.Test();

            //int a = 1;
            //Console.WriteLine(JsonConvert.SerializeObject(a));// output 1

            // log test
            // LoggerTest.MainTest();
            // Log4NetTest.MainTest();

            //ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();
            //LogHelper.AddMicrosoftLogging(loggerFactory);
            //var logger = new Logger<Program>(loggerFactory);
            //logger.LogInformation("Logging information from Microsoft.Extensions.Logging");

            InvokeHelper.TryInvoke(DataExtensionTest.MainTest);

            //TaskTest.TaskWhenAllTest().GetAwaiter().GetResult();

            //Base64UrlEncodeTest.MainTest();

            //var a = new { Name = "2123" };
            //var name = a.GetProperty("Name").GetValueGetter().Invoke(a);
            //Console.WriteLine(name);

            //var structTest = new TestStruct() { Name = "1233" };
            //var obj = (object)structTest;
            //Console.WriteLine(structTest.GetProperty("Name").GetValueGetter<TestStruct>().Invoke(structTest));
            //structTest.GetProperty("Name").GetValueSetter().Invoke(obj, "Name1");
            //structTest = (TestStruct)obj;

            //Console.WriteLine(structTest.Name);

            //Expression<Func<TestEntity, bool>> exp = t => t.Id > 10 && t.Token == "123" && t.Token.Contains("12");
            //var str = SqlExpressionParser.ParseExpression(exp);
            //Console.WriteLine("sql: {0}", str);

            //exp = t => true;
            //str = SqlExpressionParser.ParseExpression(exp);
            //Console.WriteLine("sql: {0}", str);

            //RepositoryTest.MainTest();
            //var securityToken = ApplicationHelper.ApplicationName + "test_123";
            //var code123 = TotpHelper.GenerateCode(securityToken);
            //Console.WriteLine(code123);

            //var result = TotpHelper.ValidateCode(securityToken, code123);
            //Console.WriteLine($"validate result: {result}");
            //var ttl = 2;
            //while (ttl > 1)
            //{
            //    ttl = TotpHelper.TTL(securityToken);
            //    Console.WriteLine($"Current ttl: {ttl}, newId: {ObjectIdGenerator.Instance.NewId()}");
            //    Thread.Sleep(1000);
            //}
            //result = TotpHelper.ValidateCode(securityToken, code123);
            //Console.WriteLine($"validate result: {result}");
            //var code1234 = TotpHelper.GenerateCode(ApplicationHelper.ApplicationName + "test_1234");
            //Console.WriteLine(code1234);

            Console.ReadLine();
        }

        private struct TestStruct
        {
            public string Name { get; set; }
        }

        private class TestClass
        {
            public string Name { get; set; }
        }
    }
}
