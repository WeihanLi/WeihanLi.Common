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
                container.AddTransient<WuKong>();
                container.AddScoped<WuJing>(serviceProvider => new WuJing());

                var rootConfig = container.ResolveService<IConfiguration>();
                try
                {
                    container.ResolveService<IFly>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

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

                    var number = config.GetAppSetting<int>("Number");
                    Console.WriteLine(number);
                }
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
