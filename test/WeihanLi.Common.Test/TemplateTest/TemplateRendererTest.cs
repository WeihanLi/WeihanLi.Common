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
        builder.UseTemplatePipe(new SubstringTemplatePipe());
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
    
    [Fact]
    public async Task CustomPipeRenderTest()
    {
        var name = "mike";
        var text = "Hello {{ Name | substr:2 | toTitle }}";
        var renderedText = await _templateEngine.RenderAsync(text, new { Name = name });
        Assert.Equal($"Hello {name[2..].ToTitleCase()}", renderedText);
    }
}

file sealed class SubstringTemplatePipe : TemplatePipeBase
{
    protected override int? ParameterCount => null;
    public override string Name => "substr";
    protected override string? ConvertInternal(object? value, params ReadOnlySpan<string> args)
    {
        if (args.Length is not 1 or 2)
        {
            throw new InvalidOperationException("Arguments count must be 1 or 2");
        }
        
        var str = value as string ?? value?.ToString() ?? string.Empty;
        var start = int.Parse(args[0]);
        if (args.Length is 1)
        {
            return str[start..];
        }
        var len = int.Parse(args[1]);
        return str[start..len];
    }
}
