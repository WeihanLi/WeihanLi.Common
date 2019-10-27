using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using WeihanLi.Common.DependencyInjection;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class DependencyInjectionTest : IDisposable
    {
        private readonly IServiceContainer _container;

        public DependencyInjectionTest()
        {
            _container = new ServiceContainer();

            _container.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            _container.AddScoped<IFly, MonkeyKing>();
            _container.AddScoped<IFly, Superman>();

            _container.AddScoped<HasDependencyTest>();
            _container.AddScoped<HasDependencyTest1>();
            _container.AddScoped<HasDependencyTest2>();
            _container.AddScoped<HasDependencyTest3>();
            _container.AddScoped(typeof(HasDependencyTest4<>));

            _container.AddTransient<WuKong>();
            _container.AddScoped<WuJing>(serviceProvider => new WuJing());
            _container.AddSingleton(typeof(GenericServiceTest<>));
        }

        [Fact]
        public void Test()
        {
            var rootConfig = _container.ResolveService<IConfiguration>();

            Assert.Throws<InvalidOperationException>(() => _container.ResolveService<IFly>());
            Assert.Throws<InvalidOperationException>(() => _container.ResolveRequiredService<IDependencyResolver>());

            using (var scope = _container.CreateScope())
            {
                var config = scope.ResolveService<IConfiguration>();

                Assert.Equal(rootConfig, config);

                var fly1 = scope.ResolveRequiredService<IFly>();
                var fly2 = scope.ResolveRequiredService<IFly>();
                Assert.Equal(fly1, fly2);

                var wukong1 = scope.ResolveRequiredService<WuKong>();
                var wukong2 = scope.ResolveRequiredService<WuKong>();

                Assert.NotEqual(wukong1, wukong2);

                var wuJing1 = scope.ResolveRequiredService<WuJing>();
                var wuJing2 = scope.ResolveRequiredService<WuJing>();

                Assert.Equal(wuJing1, wuJing2);

                var s0 = scope.ResolveRequiredService<HasDependencyTest>();
                s0.Test();
                Assert.Equal(s0._fly, fly1);

                var s1 = scope.ResolveRequiredService<HasDependencyTest1>();
                s1.Test();

                var s2 = scope.ResolveRequiredService<HasDependencyTest2>();
                s2.Test();

                var s3 = scope.ResolveRequiredService<HasDependencyTest3>();
                s3.Test();

                var s4 = scope.ResolveRequiredService<HasDependencyTest4<string>>();
                s4.Test();

                using (var innerScope = scope.CreateScope())
                {
                    var config2 = innerScope.ResolveRequiredService<IConfiguration>();
                    Assert.True(rootConfig == config2);

                    var fly3 = innerScope.ResolveRequiredService<IFly>();
                    fly3.Fly();

                    Assert.NotEqual(fly1, fly3);
                }

                var flySvcs = scope.ResolveServices<IFly>();
                foreach (var f in flySvcs)
                    f.Fly();
            }

            var genericService1 = _container.ResolveRequiredService<GenericServiceTest<int>>();
            genericService1.Test();

            var genericService2 = _container.ResolveRequiredService<GenericServiceTest<string>>();
            genericService2.Test();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        #region Private services

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

        private interface IFly
        {
            string Name { get; }

            void Fly();
        }

        private class MonkeyKing : IFly, IDisposable
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

        private class Superman : IFly
        {
            public string Name => "Superman";

            public void Fly()
            {
                Console.WriteLine("Superman is flying");
            }
        }

        #endregion Private services
    }
}
