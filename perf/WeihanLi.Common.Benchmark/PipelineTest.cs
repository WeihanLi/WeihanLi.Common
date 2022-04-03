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

    [Benchmark]
    public async Task ValueTaskPipeline()
    {
        var builder = PipelineBuilder.CreateValueAsync<TestContext>();
        for(var i = 0;  i < 100; i++)
        {
            builder.Use(async (context, next) =>
            {
                Debug.WriteLine(context.GetHashCode());
                await next();
            });
        }
        var pipeline = builder.Build();

        var context = new TestContext();
        await pipeline(context);
    }

    [Benchmark]
    public async Task ValueTaskPipelineInvoke()
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
        for (var i = 0; i < 100000; i++)
        {
            var context = new TestContext();
            await pipeline(context);
        }
    }

    [Benchmark]
    public async Task TaskPipeline()
    {
        var builder = PipelineBuilder.CreateAsync<TestContext>();
        for(var i = 0;  i < 100; i++)
        {
            builder.Use(async (context, next) =>
            {
                Debug.WriteLine(context.GetHashCode());
                await next();
            });
        }
        var pipeline = builder.Build();

        var context = new TestContext();
        await pipeline(context);
    }

    
    [Benchmark]
    public async Task TaskPipelineInvoke()
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
        for (var i = 0; i < 100000; i++)
        {
            var context = new TestContext();
            await pipeline(context);
        }
    }
}
