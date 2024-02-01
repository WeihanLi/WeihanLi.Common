// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreSample;
internal class ServiceDecoratorTest
{
    public static void MainTest()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJob, Sleepy>();
        services.Decorate<IJob, JobDecorator>();
        using var sp = services.BuildServiceProvider();

        var job = sp.GetRequiredService<IJob>();
        Console.WriteLine(job.Name);
        job.Execute();
    }

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
}
