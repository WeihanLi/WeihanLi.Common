// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class ConsoleHelperTest
{
    [Fact]
    public void ErrorWriteWithColorHandlesNullOutput()
    {
        using var consoleOutput = ConsoleOutput.Capture();
        
        ConsoleHelper.ErrorWriteWithColor(null, ConsoleColor.Red);

        var output = consoleOutput.StandardError;
        Assert.Empty(output);
    }

    [Fact]
    public void ErrorWriteLineWithColorHandlesNullOutput()
    {
        using var consoleOutput = ConsoleOutput.Capture();
        ConsoleHelper.ErrorWriteLineWithColor(null, ConsoleColor.Red);
        var output = consoleOutput.StandardError;
        Assert.Equal(Environment.NewLine, output);
    }

    [Fact]
    public void SupportsAnsiColorsReturnsSameValueOnMultipleCalls()
    {
        var firstCall = ConsoleHelper.SupportsAnsiColors();
        var secondCall = ConsoleHelper.SupportsAnsiColors();
        Assert.Equal(firstCall, secondCall);
    }
}
