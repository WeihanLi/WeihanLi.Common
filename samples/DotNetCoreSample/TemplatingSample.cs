﻿// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.Template;

namespace DotNetCoreSample;

public static class TemplatingSample
{
    public static async Task MainTest()
    {
        {
            var engine = TemplateEngine.CreateDefault();
            var result = await engine.RenderAsync("Hello {{Name}}", new { Name = ".NET" });
            Console.WriteLine(result);
            Console.WriteLine(await engine.RenderAsync("Hello {{Name | toTitle }}", new { Name = "mike" }));
            Console.WriteLine(await engine.RenderAsync("Today is {{ date | format:yyyy-MM-dd }}", new { date = DateTime.Today }));
        }

        {
            var result = await TemplateEngine.CreateDefault().RenderAsync("Hello {{$env USERNAME}}");
            Console.WriteLine(result);
        }

        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection([new("UserName", "Test")])
                .Build();
            var result = await TemplateEngine.CreateDefault(builder => builder.ConfigureOptions(options => options.Configuration = configuration))
                .RenderAsync("Hello {{$config UserName}}");
            Console.WriteLine(result);
        }

        {
            var services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection([new("UserName1", "Test1234")])
                .Build();
            services.AddSingleton(configuration);
            services.AddTemplateEngine();
            await using var provider = services.BuildServiceProvider();
            var result = await provider.GetRequiredService<ITemplateEngine>()
                .RenderAsync("Hello {{$config UserName1}}");
            Console.WriteLine(result);
        }
    }
}
