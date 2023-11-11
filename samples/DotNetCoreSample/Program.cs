﻿using System.Net.Mime;
// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using DotNetCoreSample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;
using WeihanLi.Extensions.Dump;

Console.WriteLine("----------DotNetCoreSample----------");

// ServiceDecoratorTest.MainTest();

// var dataLogger = LogHelper.GetLogger(typeof(DataExtension));
// DataExtension.CommandLogAction = msg => dataLogger.Debug(msg);

//AspectTest.ServiceContainerTest();

//ProcessExecutorTest.RawProcessTest();
//ProcessExecutorTest.DotNetInfoTest();
// ProcessExecutorTest.DotNetNugetGlobalPackagesInfoTest();
// PipelineTest.TestV2();

// new[] { 1, 2, 3 }.GetCombinations(2).Dump();
// new[] { 1, 2, 3 }.GetCombinations(2, true).Dump();
//
// new[] { 1, 2, 3, 2 }.GetPermutations().Dump();
// new[] { 1, 2, 3, 2 }.GetPermutations(true).Dump();
//

// CommandExecutorTest.MainTest();

//LoggerTest.MainTest();
// LoggerTest.MicrosoftLoggingTest();

// Console.ReadLine();

// var services = new ServiceCollection();
// services.AddTransient<IFly, MonkeyKing>();
// IConfiguration configuration = new ConfigurationBuilder()
//     .AddJsonFile("appsettings.json")
//     .Build();
//
// var city = configuration.GetAppSetting("City");
// var number = configuration.GetAppSetting<int>("Number");
// Console.WriteLine($"City:{city}, Number:{number}");
//
// services.AddSingleton(configuration);

//services.AddSingleton<IEventStore, EventStoreInMemory>();
//services.AddSingleton<IEventBus, EventBus>();

//services.AddSingleton(DelegateEventHandler.FromAction<CounterEvent>(@event =>
//    LogHelper.GetLogger(typeof(DelegateEventHandler<CounterEvent>))
//        .Info($"Event Info: {@event.ToJson()}")
//    )
//);
//
// services.AddSingletonProxy<IFly, MonkeyKing>();
//
// Action<DbContextOptionsBuilder> dbContextOptionsAction = options =>
// {
//     options.UseInMemoryDatabase("Test");
// };
// services.AddDbContext<TestDbContext>(dbContextOptionsAction);
//
// services.AddScopedProxy<TestDbContext>();
// services.AddEvents();
//
// services.AddSingletonProxy<IEventBus, EventBus>();
// services.AddFluentAspects(options =>
//     {
//         options.NoInterceptPropertyGetter<IFly>(f => f.Name);
//
//         options.InterceptAll()
//             .With<LogInterceptor>()
//             ;
//         options.InterceptMethod<DbContext>(x => x.Name == nameof(DbContext.SaveChanges)
//                                                 || x.Name == nameof(DbContext.SaveChangesAsync))
//             .With<DbContextSaveInterceptor>()
//             ;
//         options.InterceptMethod<IFly>(f => f.Fly())
//             .With<LogInterceptor>();
//         options.InterceptType<IFly>()
//             .With<LogInterceptor>();
//
//         options
//             .WithProperty("TraceId", "121212")
//             ;
//     })
//     // .UseCastleProxy()
//     // .UseAspectCoreProxy()
//     ;
//
// DependencyResolver.SetDependencyResolver(services);

//var fly = DependencyResolver.ResolveService<IFly>();
//Console.WriteLine(fly.Name);
//fly.Fly();
//fly.OpenFly<int>();
//fly.OpenFly<string>();
//fly.FlyAway();

//var animal1 = FluentAspects.AspectOptions.ProxyFactory.CreateInterfaceProxy<IAnimal<int>>();
//animal1.Eat();

//var animal2 = FluentAspects.AspectOptions.ProxyFactory.CreateInterfaceProxy<IAnimal<string>>();
//animal2.Eat();

//var animal = FluentAspects.AspectOptions.ProxyFactory.CreateProxy<Animal<string>>();
//animal.Eat();
//animal.Eat();
//Console.WriteLine(animal.GetEatCount());
//animal.Drink("xxx");
//Console.WriteLine(animal.GetDrinkCount());

//animal = FluentAspects.AspectOptions.ProxyFactory.CreateProxyWithTarget<Animal<string>>(new Animal<string>());
//animal.Eat();
//animal.Eat();
//Console.WriteLine(animal.GetEatCount());
//animal.Drink("xxx");
//Console.WriteLine(animal.GetDrinkCount());

//DependencyResolver.TryInvokeService<TestDbContext>(dbContext =>
//{
//    dbContext.TestEntities.Add(new TestEntity() { Token = "sasa", CreatedTime = DateTime.Now, });
//    var hasChanges = dbContext.ChangeTracker.HasChanges();
//    Console.WriteLine($"hasChanges：{hasChanges}");
//    dbContext.SaveChanges();
//});

//DependencyResolver.TryInvokeService<IEventBus>(eventBus =>
//{
//    eventBus.Publish(new CounterEvent());
//});

// DependencyResolver.TryInvoke<IProxyFactory>(proxyFactory =>
// {
//     var counterEvent = new CounterEvent() { Counter = 1 };
//     var eventBusProxy = proxyFactory.CreateProxy<IEventBus, EventBus>();
//     eventBusProxy.Publish(counterEvent);
//
//     var handlerProxy = proxyFactory.CreateProxyWithTarget<IEventHandler<CounterEvent>>(DelegateEventHandler.FromAction<CounterEvent>(e =>
//     {
//         Console.WriteLine(e.ToJson());
//     }));
//     handlerProxy.Handle(counterEvent)
//         .GetAwaiter().GetResult();
// });

//DependencyResolver.TryInvokeServiceAsync<TestDbContext>(dbContext =>
//{
//    dbContext.TestEntities.Add(new TestEntity() { Token = "sasa", CreatedTime = DateTime.Now, });

//    if (dbContext.ChangeTracker is null)
//    {
//        Console.WriteLine($"{nameof(dbContext.ChangeTracker)} is null ...");
//    }
//    else
//    {
//        foreach (var entry in dbContext.ChangeTracker.Entries<TestEntity>())
//        {
//            Console.WriteLine(entry.Entity.Token);
//        }
//    }

//    if (dbContext.Database is null)
//    {
//        Console.WriteLine($"{nameof(dbContext.Database)} is null ...");
//    }
//    if (dbContext.Database is null)
//    {
//        Console.WriteLine($"{nameof(dbContext.Database)} is null ...");
//    }

//    return dbContext.SaveChangesAsync();
//});

//DependencyResolver.ResolveRequiredService<IFly>()
//    .Fly();

//DependencyInjectionTest.Test();

// EventTest.MainTest();

// SerilogTest.MainTest();

//var builder = new ContainerBuilder();
//builder.RegisterType<MonkeyKing>().As<IFly>();

//DependencyResolver.SetDependencyResolver((IServiceProvider)new AutofacDependencyResolver(builder.Build()));

//DependencyInjectionTest.Test();

//int a = 1;
//Console.WriteLine(JsonConvert.SerializeObject(a));// output 1

//var pagedListModel = new PagedListModel<int>()
//{
//    PageNumber = 1,
//    PageSize = 4,
//    TotalCount = 10,
//    Data = new[] { 1, 2, 3, 4 }
//};
//Console.WriteLine(pagedListModel.ToJson());

// log test
// LoggerTest.MainTest();

//ILoggerFactory loggerFactory = new LoggerFactory();
//loggerFactory.AddConsole();
//loggerFactory.AddDebug();
//loggerFactory.AddDelegateLogger(
//    (category, logLevel, exception, msg) =>
//    {
//        Console.WriteLine($"{category}:[{logLevel}] {msg}\n{exception}");
//    }
//);

//var logger = new Logger<Program>(loggerFactory);
//logger.LogInformation("Logging information from Microsoft.Extensions.Logging");

//InvokeHelper.TryInvoke(DataExtensionTest.MainTest);

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

//var securityToken = ApplicationHelper.ApplicationName + "_test_123";
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
//Console.WriteLine($"validate result1: {result}");

//result = TotpHelper.ValidateCode(securityToken, code123, 60);
//Console.WriteLine($"validate result2: {result}");
//var code1234 = TotpHelper.GenerateCode(ApplicationHelper.ApplicationName + "test_1234");
//Console.WriteLine(code1234);

// InvokeHelper.TryInvoke(HttpRequesterTest.MainTest);

//var pagedListModel = new PagedListModel<int>()
//{
//    PageNumber = 2, PageSize = 2, TotalCount = 6, Data = new int[] {1, 2},
//};
//var pagedListModel1 = new PagedListModel1<int>()
//{
//    PageNumber = 2,
//    PageSize = 2,
//    TotalCount = 6,
//    Data = new int[] { 1, 2 },
//};
//Console.WriteLine($"pagedListModel:{JsonConvert.SerializeObject(pagedListModel)}, pagedListModel1:{JsonConvert.SerializeObject(pagedListModel1)}");

//var posts = new[] { new { PostId = 1, PostTitle = "12333", }, new { PostId = 2, PostTitle = "12333", }, };
//var postTags = new[] { new { PostId = 1, Tag = "HHH" } };

//var result = posts.LeftJoin(postTags, p => p.PostId, pt => pt.PostId, (p, pt) => new { p.PostId, p.PostTitle, pt?.Tag }).ToArray();
//Console.WriteLine(result.ToJson());

// CronHelperTest.MainTest();

// DependencyInjectionTest.BuiltInIocTest();

//Expression<Func<TestEntity, bool>> expression =
//    t => t.Id > 0 && t.CreatedTime < DateTime.Now && t.Token == null;
//var visitor = new SqlExpressionVisitor(null);
//visitor.Visit(expression);
//Console.WriteLine(visitor.GetCondition());

//PipelineTest.TestV2();
//PipelineTest.AsyncPipelineBuilderTestV2().Wait();

//TotpTest.MainTest();

// exit test
// var exitToken = InvokeHelper.GetExitToken();
// exitToken.Register(() =>
// {
//     Console.WriteLine(@"Exiting");
//     Thread.Sleep(3000);
//     Console.WriteLine(@"Exited");
// });
// ApplicationHelper.RuntimeInfo.Dump();
Console.WriteLine(ApplicationHelper.ResolvePath("yarn.cmd"));
await AppHostTest.MainTest();
// NewtonJsonFormatterTest.MainTest();

// ConsoleHelper.ReadKeyWithPrompt("Press any key to exit");

internal struct TestStruct
{
    public string? Name { get; set; }
}

internal class TestClass
{
    public string? Name { get; set; }
}
