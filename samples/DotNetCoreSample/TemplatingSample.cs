// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using WeihanLi.Common.Templating;

namespace DotNetCoreSample;

public class TemplatingSample
{
    public static async Task MainTest()
    {
        {
            var engine = TemplateEngine.CreateDefault();
            var result = await engine.RenderAsync("Hello {{Name}}", new { Name = ".NET" });
            Console.WriteLine(result);
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
    }
}
