// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;

namespace DotNetCoreSample;

public class DisposeTest
{
    public static void MainTest()
    {
        Console.WriteLine(@$"---- {nameof(MainTest)} start");
        {
            using var service = new TestService()
            {
                Name = "MainTest"
            };
        }
        {
            var service = new TestService()
            {
                Name = "MainTest1"
            };
            service.Dispose();
        }
        {
            // forget to dispose
            var service = new TestService()
            {
                Name = "MainTest2"
            };
            Console.WriteLine(service.GetType());
            
            service = null;
            Console.WriteLine(service is null);
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine(@$"---- {nameof(MainTest)} end");
    }
    
    public static async ValueTask MainTestAsync()
    {
        Console.WriteLine(@$"---- {nameof(MainTestAsync)} start");
        {
            await using var service = new TestService()
            {
                Name = "MainTestAsync"
            };
        }
        {
            var service = new TestService()
            {
                Name = "MainTestAsync1"
            };
            await service.DisposeAsync();
        }
        {
            // forget to dispose
            var service = new TestService()
            {
                Name = "MainTestAsync2"
            };
            Console.WriteLine(service.GetType());
            
            service = null;
            Console.WriteLine(service is null);
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine(@$"---- {nameof(MainTestAsync)} end");
    }
}

file sealed class TestService : DisposableBase
{
    public required string Name { get; init; }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Console.WriteLine($@"<<{Name}>> disposes managed resources");    
        }
        Console.WriteLine($@"<<{Name}>> disposes unmanaged resources");
        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        {
            Console.WriteLine($@"<<{Name}>> disposes managed resources async");
            await Task.Yield();
        }
        await base.DisposeAsyncCore();
    }

    ~TestService()
    {
        Console.WriteLine($@"<<{Name}>> Finalizer executing");
    }
}
