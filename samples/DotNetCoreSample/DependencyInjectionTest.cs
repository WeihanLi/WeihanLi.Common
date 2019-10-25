using Microsoft.Extensions.Configuration;
using System;
using WeihanLi.Common;
using WeihanLi.Common.DependencyInjection;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetCoreSample
{
    internal class DependencyInjectionTest
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<DependencyInjectionTest>();

        public static void Test()
        {
            var fly = DependencyResolver.Current.ResolveService<IFly>();
            Console.WriteLine(fly.Name);
            fly.Fly();

            DependencyResolver.Current.TryInvokeService<IFly>(f =>
            {
                Console.WriteLine(f.Name);
                f.Fly();
            });
        }

        public static void BuiltInIocTest()
        {
            using (IServiceContainer container = new ServiceContainer())
            {
                container.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                );
                container.AddScoped<IFly, MonkeyKing>();
                container.AddScoped<IFly, Superman>();

                container.AddScoped<HasDependencyTest>();
                container.AddTransient<WuKong>();
                container.AddScoped<WuJing>(serviceProvider => new WuJing());
                container.AddSingleton(typeof(GenericServiceTest<>));

                var rootConfig = container.ResolveService<IConfiguration>();
                //try
                //{
                //    container.ResolveService<IFly>();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //}

                using (var scope = container.CreateScope())
                {
                    var config = scope.ResolveService<IConfiguration>();
                    var fly1 = scope.ResolveService<IFly>();
                    var fly2 = scope.ResolveService<IFly>();

                    var wukong1 = scope.ResolveService<WuKong>();
                    var wukong2 = scope.ResolveService<WuKong>();

                    var wuJing1 = scope.ResolveService<WuJing>();
                    var wuJing2 = scope.ResolveService<WuJing>();

                    Console.WriteLine("fly1 == fly2,  {0}", fly1 == fly2);
                    Console.WriteLine("rootConfig == config, {0}", rootConfig == config);
                    Console.WriteLine("wukong1 == wukong2, {0}", wukong1 == wukong2);
                    Console.WriteLine("wujing1 == wujing2, {0}", wuJing1 == wuJing2);

                    fly1.Fly();

                    wukong1.Jump();
                    wukong2.Jump();

                    wuJing1.Eat();

                    var s1 = scope.ResolveRequiredService<HasDependencyTest>();
                    s1.Test();

                    Console.WriteLine($"s1._fly == fly1 : {s1._fly == fly1}");

                    using (var innerScope = scope.CreateScope())
                    {
                        var config2 = innerScope.ResolveRequiredService<IConfiguration>();
                        Console.WriteLine("rootConfig == config2, {0}", rootConfig == config2);
                        var fly3 = innerScope.ResolveRequiredService<IFly>();
                        fly3.Fly();
                    }

                    var number = config.GetAppSetting<int>("Number");
                    Console.WriteLine(number);

                    var flySvcs = scope.ResolveServices<IFly>();
                    foreach (var f in flySvcs)
                        f.Fly();
                }

                var genericService1 = container.ResolveRequiredService<GenericServiceTest<int>>();
                genericService1.Test();

                var genericService2 = container.ResolveRequiredService<GenericServiceTest<string>>();
                genericService2.Test();
            }
        }

        private class WuKong : IDisposable
        {
            public WuKong()
            {
                Console.WriteLine("wukong initialized");
            }

            public void Jump()
            {
                Console.WriteLine("wukong jumped 一万八千里");
            }

            public void Dispose()
            {
                Console.WriteLine("wukong disposed");
            }
        }

        private class WuJing : IDisposable
        {
            public WuJing()
            {
                Console.WriteLine("WuJing initialized");
            }

            public void Eat()
            {
                Console.WriteLine("WuJing eated balaba.....");
            }

            public void Dispose()
            {
                Console.WriteLine("WuJing disposed");
            }
        }

        private class GenericServiceTest<T>
        {
            public void Test()
            {
                Console.WriteLine($"generic type: {typeof(T).FullName}");
            }
        }

        private class HasDependencyTest
        {
            public readonly IFly _fly;

            public HasDependencyTest(IFly fly)
            {
                _fly = fly;
            }

            public void Test()
            {
                Console.WriteLine($"test in {nameof(HasDependencyTest)}");
                _fly.Fly();
            }
        }
    }

    internal interface IFly
    {
        string Name { get; }

        void Fly();
    }

    internal class MonkeyKing : IFly, IDisposable
    {
        public string Name => "MonkeyKing";

        public void Fly()
        {
            Console.WriteLine($"{Name} is flying");
        }

        public void Dispose()
        {
            Console.WriteLine($"{Name}  disposed...");
        }
    }

    internal class Superman : IFly
    {
        public string Name => "Superman";

        public void Fly()
        {
            Console.WriteLine("Superman is flying");
        }
    }
}
