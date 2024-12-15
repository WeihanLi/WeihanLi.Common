// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Template;
using Xunit;

namespace WeihanLi.Common.Test.TemplateTest;

public class TemplateParserTest
{
    [Fact]
    public async Task PipeParseTest()
    {
        var text = "Hello {{Name | upper}}";
        var parser = new DefaultTemplateParser();
        var context = await parser.ParseAsync(text);
        Assert.Single(context.Inputs);
        Assert.Single(context.Inputs.Keys.First().Pipes);
        Assert.Single(context.Inputs.Keys.First().Pipes);
        var input = context.Inputs.Keys.First().Input;
        var pipe = context.Inputs.Keys.First().Pipes.First();
        Assert.Equal("{{Name | upper}}", input);
        Assert.Equal("upper", pipe.PipeName);
        Assert.NotNull(pipe.Arguments);
        Assert.Empty(pipe.Arguments);
    }

    [Fact]
    public async Task ParseTest()
    {
        var template = """
                Build Status: {{Status}}
                Chart: {{$env CHART_NAME}} - {{$env VERSION}}
                AppVersion: {{$env APP_VERSION}}
                HostEnv: {{$env HOST | toUpper }}
                Config: {{$config AppSettings:Host}}
                Config: {{$config AppSettings--Host}}
                Config: {{$config AppSettings__Host}}
                """;
        var result = await new DefaultTemplateParser()
            .ParseAsync(template);
        var inputs = result.Inputs.Select(x => x.Key.Input)
            .ToHashSet();
        Assert.Equal(7, result.Inputs.Count);
        Assert.Contains("{{Status}}", inputs);
        Assert.Contains("{{$env CHART_NAME}}", inputs);
        Assert.Contains("{{$env VERSION}}", inputs);
        Assert.Contains("{{$env APP_VERSION}}", inputs);
        Assert.Contains("{{$env HOST | toUpper }}", inputs);
        Assert.Contains("{{$config AppSettings:Host}}", inputs);
        Assert.Contains("{{$config AppSettings__Host}}", inputs);

        var variableNames = result.Inputs.Select(x => x.Key.VariableName).ToHashSet();
        Assert.Contains("Status", variableNames);
        Assert.Contains("CHART_NAME", variableNames);
        Assert.Contains("VERSION", variableNames);
        Assert.Contains("APP_VERSION", variableNames);
        Assert.Contains("AppSettings:Host", variableNames);
        Assert.Contains("AppSettings__Host", variableNames);
        Assert.Contains("HOST", variableNames);

        var pipes = result.Inputs.SelectMany(x => x.Key.Pipes)
            .Select(x => x.PipeName)
            .ToArray();
        Assert.Contains("toUpper", pipes);
    }
}
