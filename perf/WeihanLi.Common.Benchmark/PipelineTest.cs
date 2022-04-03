// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Benchmark;

[MemoryDiagnoser]
public class PipelineTest
{
    private sealed class TestContext
    {
    }

    [Benchmark(Baseline = true)]
    public async Task ValueTaskPipeline()
    {
        var builder = PipelineBuilder.CreateValueAsync<TestContext>();
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        var pipeline = builder.Build();
        for (var i = 0; i < 10000; i++)
        {
            var context = new TestContext();
            await pipeline(context);
        }
    }

    [Benchmark]
    public async Task TaskPipeline()
    {
        var builder = PipelineBuilder.CreateAsync<TestContext>();
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        builder.Use(async (context, next) =>
        {
            Debug.WriteLine(context.GetHashCode());
            await next();
        });
        var pipeline = builder.Build();
        for (var i = 0; i < 10000; i++)
        {
            var context = new TestContext();
            await pipeline(context);
        }
    }
}
