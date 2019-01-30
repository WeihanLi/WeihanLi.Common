using System;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Log;

namespace DotNetCoreSample
{
    internal class DependencyInjectionTest
    {
        private static readonly ILogHelper Logger = LogHelper.GetLogHelper<DependencyInjectionTest>();

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
            Console.WriteLine("MonkeyKing is flying");
        }

        public void Dispose()
        {
            Console.WriteLine("MonkeyKing is disposing..");
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
