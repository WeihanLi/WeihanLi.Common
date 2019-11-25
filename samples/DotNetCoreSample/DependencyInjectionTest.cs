using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IServiceContainerBuilder containerBuilder = new ServiceContainerBuilder();
            containerBuilder.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
            );
            containerBuilder.AddScoped<IFly, MonkeyKing>();
            containerBuilder.AddScoped<IFly, Superman>();

            containerBuilder.AddScoped<HasDependencyTest>();
            containerBuilder.AddScoped<HasDependencyTest1>();
            containerBuilder.AddScoped<HasDependencyTest2>();
            containerBuilder.AddScoped<HasDependencyTest3>();
            containerBuilder.AddScoped(typeof(HasDependencyTest4<>));

            containerBuilder.AddTransient<WuKong>();
            containerBuilder.AddScoped<WuJing>(serviceProvider => new WuJing());
            containerBuilder.AddSingleton(typeof(GenericServiceTest<>));

            containerBuilder.RegisterAssemblyModules();

            using (var container = containerBuilder.Build())
            {
                var idGenerator = container.ResolveRequiredService<IIdGenerator>();
                Console.WriteLine(idGenerator.NewId());
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

                    var s0 = scope.ResolveRequiredService<HasDependencyTest>();
                    s0.Test();
                    Console.WriteLine($"s0._fly == fly1 : {s0._fly == fly1}");

                    var s1 = scope.ResolveService<HasDependencyTest1>();
                    s1.Test();

                    var s2 = scope.ResolveService<HasDependencyTest2>();
                    s2.Test();

                    var s3 = scope.ResolveService<HasDependencyTest3>();
                    s3.Test();

                    var s4 = scope.ResolveService<HasDependencyTest4<string>>();
                    s4.Test();

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

        private class HasDependencyTest1
        {
            private readonly IReadOnlyCollection<IFly> _flys;

            public HasDependencyTest1(IEnumerable<IFly> flys)
            {
                _flys = flys.ToArray();
            }

            public void Test()
            {
                Console.WriteLine($"test in {nameof(HasDependencyTest1)}");
                foreach (var item in _flys)
                {
                    item.Fly();
                }
            }
        }

        private class HasDependencyTest2
        {
            private readonly IReadOnlyCollection<IFly> _flys;

            public HasDependencyTest2(IReadOnlyCollection<IFly> flys)
            {
                _flys = flys;
            }

            public void Test()
            {
                Console.WriteLine($"test in {nameof(HasDependencyTest2)}");
                foreach (var item in _flys)
                {
                    item.Fly();
                }
            }
        }

        private class HasDependencyTest3
        {
            private readonly IReadOnlyCollection<GenericServiceTest<int>> _svcs;

            public HasDependencyTest3(IEnumerable<GenericServiceTest<int>> svcs)
            {
                _svcs = svcs.ToArray();
            }

            public void Test()
            {
                Console.WriteLine($"test in {nameof(HasDependencyTest3)}");
                foreach (var item in _svcs)
                {
                    item.Test();
                }
            }
        }

        private class HasDependencyTest4<T>
        {
            private readonly IReadOnlyCollection<GenericServiceTest<T>> _svcs;

            public HasDependencyTest4(IEnumerable<GenericServiceTest<T>> svcs)
            {
                _svcs = svcs.ToArray();
            }

            public void Test()
            {
                Console.WriteLine($"test in {GetType().FullName}");
                foreach (var item in _svcs)
                {
                    item.Test();
                }
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

    internal class TestServiceContainerModule : ServiceContainerModule
    {
        public override void ConfigureServices(IServiceContainerBuilder serviceContainerBuilder)
        {
            serviceContainerBuilder.AddSingleton<IIdGenerator>(GuidIdGenerator.Instance);
        }
    }
}
