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
                """;
        var result = await new DefaultTemplateParser()
            .ParseAsync(template);
        Assert.Equal(4, result.Variables.Count);
    }
}
