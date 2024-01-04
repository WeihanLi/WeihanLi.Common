// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Template;
using Xunit;

namespace WeihanLi.Common.Test.TemplateTest;

public class TemplateParserTest
{
    [Fact]
    public async Task ParseTest()
    {
        var template = """
                Build Status: {{Status}}
                Chart: {{$env CHART_NAME}} - {{$env VERSION}}
                AppVersion: {{$env APP_VERSION}}
                Config: {{$config AppSettings:Host}}
                Config: {{$config AppSettings--Host}}
                Config: {{$config AppSettings__Host}}
                """;
        var result = await new DefaultTemplateParser()
            .ParseAsync(template);
        Assert.Equal(6, result.Variables.Count);
        Assert.Contains("Status", result.Variables);
        Assert.Contains("$env CHART_NAME", result.Variables);
        Assert.Contains("$env VERSION", result.Variables);
        Assert.Contains("$env APP_VERSION", result.Variables);
        Assert.Contains("$config AppSettings:Host", result.Variables);
        Assert.Contains("$config AppSettings__Host", result.Variables);
    }
}
