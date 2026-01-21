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
        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.ErrorWriteWithColor(null, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardError;
        Assert.Empty(output);
    }

    [Fact]
    public void ErrorWriteLineWithColorHandlesNullOutput()
    {
        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.ErrorWriteLineWithColor(null, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardError;
        Assert.Equal(Environment.NewLine, output);
    }

    [Fact]
    public void SupportsAnsiColorsReturnsSameValueOnMultipleCalls()
    {
        // Arrange & Act
        var firstCall = ConsoleHelper.SupportsAnsiColors();
        var secondCall = ConsoleHelper.SupportsAnsiColors();
        
        // Assert
        Assert.Equal(firstCall, secondCall);
    }

    [Fact]
    public void SupportsAnsiColorsReturnsBooleanValue()
    {
        // Act
        var result = ConsoleHelper.SupportsAnsiColors();
        
        // Assert
        Assert.IsType<bool>(result);
    }
}
