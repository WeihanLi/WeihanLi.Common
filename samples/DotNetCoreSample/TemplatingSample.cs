// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Templating;

namespace DotNetCoreSample;

public class TemplatingSample
{
    public static void MainTest()
    {
        var engine = TemplateEngine.CreateDefault();
        var result = engine.RenderAsync("Hello {{Name}}", new { Name = ".NET" });
        result.Wait();
        Console.WriteLine(result.Result);
    }
}
