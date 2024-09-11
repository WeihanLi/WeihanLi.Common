// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;
public class ApplicationHelperTest
{
    [Fact]
    public void DotnetPathTest()
    {
        var dotnetPath = ApplicationHelper.GetDotnetPath();
        Assert.NotNull(dotnetPath);
        Assert.NotEmpty(dotnetPath);
        Assert.True(File.Exists(dotnetPath));
    }
}
