// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Template;
using Xunit;

namespace WeihanLi.Common.Test.TemplateTest;

public class TemplateRendererTest
{
    [Fact]
    public async Task EnvRenderTest()
    {
        var text = "Hello {{$env hostname}}";
        var renderedText = await TemplateEngine.CreateDefault().RenderAsync(text);
        Assert.Equal($"Hello {Environment.GetEnvironmentVariable("hostname")}", renderedText);
    }
}
