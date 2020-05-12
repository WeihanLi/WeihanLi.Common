using System;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;

namespace DotNetFxSample
{
    public class AspectTest
    {
        public static void MainTest()
        {
            FluentAspects.Configure(options =>
            {
                options.InterceptMethod<IFly>(f => f.Fly())
                    .With<LogInterceptor>();
                options.NoInterceptPropertyGetter<IFly>(f => f.Name);

                // options.UseInterceptorResolver<AttributeInterceptorResolver>();
            });
            //
            var fly = FluentAspects.AspectOptions.ProxyFactory.CreateProxy<IFly, MonkeyKing>();
            Console.WriteLine(fly.Name);
            fly.Fly();
        }
    }

    public class LogInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            Console.WriteLine($"the method [{invocation.ProxyMethod}] invoke begin");
            await next();
            Console.WriteLine($"the method [{invocation.ProxyMethod}] invoke end");
        }
    }

    public interface IFly
    {
        string Name { get; }

        void Fly();
    }

    public class MonkeyKing : IFly
    {
        public string Name => "SunWukong";

        public void Fly()
        {
            Console.WriteLine("the MonkeyKing is flying");
        }
    }
}
