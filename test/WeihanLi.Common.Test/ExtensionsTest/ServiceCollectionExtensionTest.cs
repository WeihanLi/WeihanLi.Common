// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;
public class ServiceCollectionExtensionTest
{
    [Fact]
    public void DecorateNonGenericType()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJob, Sleepy>();
        services.Decorate<IJob, JobDecorator>();
        using var sp = services.BuildServiceProvider();

        var job = sp.GetRequiredService<IJob>();
        Assert.IsType<JobDecorator>(job);
        Assert.Equal($"??? {nameof(Sleepy)}", job.Name);
    }

    //[Fact]
    //public void DecorateOpenGenericType()
    //{
    //    var services = new ServiceCollection();
    //    services.AddSingleton(typeof(IValueProvider<>), typeof(DefaultValueProvider<>));
    //    services.Decorate(typeof(IValueProvider<>), typeof(ValueProviderDecorator<>));
    //    using var sp = services.BuildServiceProvider();

    //    var service = sp.GetRequiredService<IValueProvider<int>>();
    //    Assert.IsType<ValueProviderDecorator<int>>(service);
    //    service.GetValue();
    //}

    private interface IJob
    {
        string Name { get; }
        void Execute();
    }

    private sealed class Sleepy : IJob
    {
        public string Name => nameof(Sleepy);

        public void Execute()
        {
            Console.WriteLine("Sleeping...");
        }
    }

    private sealed class JobDecorator(IJob job) : IJob
    {
        private readonly IJob _job = job;

        public string Name => $"??? {_job.Name}";

        public void Execute()
        {
            Console.WriteLine("Before execute");

            _job.Execute();

            Console.WriteLine("After execute");
        }
    }
    private interface IValueProvider<T>
    {
        T? GetValue();
    }
    private class DefaultValueProvider<T> : IValueProvider<T>
    {
        public T? GetValue()
        {
            return default;
        }
    }
    private sealed class ValueProviderDecorator<T>(IValueProvider<T> valueProvider) : IValueProvider<T>
    {
        private readonly IValueProvider<T> _valueProvider = valueProvider;

        public int Counter { get; private set; }

        public T? GetValue()
        {
            var value = _valueProvider.GetValue();
            Counter++;
            return value;
        }
    }
}
