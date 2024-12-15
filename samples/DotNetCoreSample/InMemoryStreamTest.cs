// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetCoreSample;
internal static class InMemoryStreamTest
{
    public static async Task MainTest()
    {
        var stream = new InMemoryStream<long>("test");
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await Task.Delay(100);
        await stream.AddAsync(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), new Dictionary<string, string>
        {
            { "messages", new { name = $"test-{DateTimeOffset.Now}" } .ToJson() }
        });
        Console.WriteLine("stream message added");
        await Task.Delay(1000);
        await stream.AddAsync(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), new Dictionary<string, string>
        {
            { "messages", new { name = $"test-{DateTimeOffset.Now}" } .ToJson() }
        });
        Console.WriteLine("stream message added");
        //
        {
            Console.WriteLine("Fetch messages from stream");
            await foreach (var item in stream.FetchAsync(timestamp, 2))
            {
                Console.WriteLine($"{item.Id} - {item.Timestamp}");
                Console.WriteLine(item.Fields.ToJson());
            }
        }
        {
            Console.WriteLine("Fetch messages from stream again");
            await foreach (var item in stream.FetchAsync(timestamp, 2))
            {
                Console.WriteLine($"{item.Id} - {item.Timestamp}");
                Console.WriteLine(item.Fields.ToJson());
            }
        }
    }
}
