// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using WeihanLi.Common.Template;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.TemplateTest;

public class TemplateRendererTest
{
    private readonly TemplateEngine _templateEngine = TemplateEngine.CreateDefault(builder =>
    {
        builder.ConfigureOptions(options =>
        {
            options.Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("Name", "test")
                ])
                .Build();
        });
    });
    
    [Fact]
    public async Task VariableRenderTest()
    {
        var name = "mike";
        var text = "Hello {{ Name | toTitle }}";
        var renderedText = await _templateEngine.RenderAsync(text, new { Name = name });
        Assert.Equal($"Hello {name.ToTitleCase()}", renderedText);
    }
    
    [Fact]
    public async Task ConfigRenderTest()
    {
        var text = "Hello {{ $config Name | toTitle }}";
        var renderedText = await _templateEngine.RenderAsync(text, new { Name = "mike" });
        Assert.Equal($"Hello {"test".ToTitleCase()}", renderedText);
    }
    
    [Fact]
    public async Task EnvRenderTest()
    {
        var text = "Hello {{$env hostname}}";
        var renderedText = await _templateEngine.RenderAsync(text);
        Assert.Equal($"Hello {Environment.GetEnvironmentVariable("hostname")}", renderedText);
    }
}
