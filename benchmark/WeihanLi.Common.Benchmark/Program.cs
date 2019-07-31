using System;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());
            DependencyResolver.SetDependencyResolver(serviceCollection);

            // BenchmarkRunner.Run<MapperTest>();
            BenchmarkRunner.Run<CreateInstanceTest>();

            Console.ReadLine();
        }
    }
}
