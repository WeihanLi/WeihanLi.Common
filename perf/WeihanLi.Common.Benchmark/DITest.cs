using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Validators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Benchmark;

// https://github.com/aspnet/DependencyInjection/blob/rel/2.0.0/test/Microsoft.Extensions.DependencyInjection.Performance/GetServiceBenchmark.cs
[Config(typeof(CoreConfig))]
public class DITest
{
    // refer to https://github.com/aspnet/DependencyInjection/blob/rel/2.0.0/test/Microsoft.Extensions.DependencyInjection.Performance/configs/CoreConfig.cs
    private class CoreConfig : ManualConfig
    {
        public CoreConfig()
        {
            AddValidator(JitOptimizationsValidator.FailOnError);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddColumn(StatisticColumn.OperationsPerSecond);
        }
    }

    private const int OperationsPerInvoke = 50000;

    private IServiceProvider _transientSp;
    private IServiceScope _scopedSp;
    private IServiceProvider _singletonSp;

    private IServiceContainer _singletonContainer;
    private IServiceContainer _scopedRootContainer;
    private IServiceContainer _scopedContainer;
    private IServiceContainer _transientContainer;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddSingleton<A>();
        services.AddSingleton<B>();
        services.AddSingleton<C>();
        _singletonSp = services.BuildServiceProvider();

        services = new ServiceCollection();
        services.AddScoped<A>();
        services.AddScoped<B>();
        services.AddScoped<C>();
        _scopedSp = services.BuildServiceProvider().CreateScope();

        services = new ServiceCollection();
        services.AddTransient<A>();
        services.AddTransient<B>();
        services.AddTransient<C>();
        _transientSp = services.BuildServiceProvider();

        var containerBuilder = new ServiceContainerBuilder();
        containerBuilder.AddSingleton<A>();
        containerBuilder.AddSingleton<B>();
        containerBuilder.AddSingleton<C>();

        _singletonContainer = containerBuilder.Build();

        containerBuilder = new ServiceContainerBuilder();
        containerBuilder.AddScoped<A>();
        containerBuilder.AddScoped<B>();
        containerBuilder.AddScoped<C>();

        _scopedRootContainer = containerBuilder.Build();
        _scopedContainer = _scopedRootContainer.CreateScope();

        containerBuilder = new ServiceContainerBuilder();
        containerBuilder.AddTransient<A>();
        containerBuilder.AddTransient<B>();
        containerBuilder.AddTransient<C>();

        _transientContainer = containerBuilder.Build();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _singletonContainer?.Dispose();
        _scopedRootContainer?.Dispose();
        _scopedContainer?.Dispose();
        _transientContainer?.Dispose();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public void NoDI()
    {
        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            var temp = new A(new B(new C()));
            temp.Foo();
        }
    }

    //[Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    //public void Reflection()
    //{
    //    for (var i = 0; i < OperationsPerInvoke; i++)
    //    {
    //        var temp = (A)Activator.CreateInstance(typeof(A),
    //                Activator.CreateInstance(typeof(B),
    //                    Activator.CreateInstance(typeof(C))))
    //            ;
    //        temp.Foo();
    //    }
    //}

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Singleton()
    {
        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            var temp = _singletonSp.GetService<A>();
            temp.Foo();
        }
    }

    //[Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    //public void Scoped()
    //{
    //    for (var i = 0; i < OperationsPerInvoke; i++)
    //    {
    //        var temp = _scopedSp.ServiceProvider.GetService<A>();
    //        temp.Foo();
    //    }
    //}

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Transient()
    {
        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            var temp = _transientSp.GetService<A>();
            temp.Foo();
        }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void ServiceContainerSingletonTest()
    {
        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            var temp = _singletonContainer.GetService<A>();
            temp.Foo();
        }
    }

    //[Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    //public void ServiceContainerScopedTest()
    //{
    //    for (var i = 0; i < OperationsPerInvoke; i++)
    //    {
    //        var temp = _scopedContainer.GetService<A>();
    //        temp.Foo();
    //    }
    //}

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void ServiceContainerTransientTest()
    {
        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            var temp = _transientContainer.GetService<A>();
            temp.Foo();
        }
    }

    private class A
    {
        public A(B b)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Foo()
        {
        }
    }

    private class B
    {
        public B(C c)
        {
        }
    }

    private class C
    {
    }
}
