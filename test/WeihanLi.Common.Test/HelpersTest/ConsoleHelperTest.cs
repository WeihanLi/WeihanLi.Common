// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class ConsoleHelperTest
{
    [Fact]
    public void WriteWithColorProducesOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test message";
        
        // Act
        ConsoleHelper.WriteWithColor(testMessage, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        Assert.Contains("m", output); // ANSI escape sequence ends with 'm'
    }

    [Fact]
    public void WriteLineWithColorProducesOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test message";
        
        // Act
        ConsoleHelper.WriteLineWithColor(testMessage, ConsoleColor.Green);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        Assert.Contains("m", output); // ANSI escape sequence ends with 'm'
    }

    [Fact]
    public void WriteWithColorHandlesNullOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.WriteWithColor(null, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Empty(output);
    }

    [Fact]
    public void WriteLineWithColorHandlesNullOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.WriteLineWithColor(null, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Equal(Environment.NewLine, output);
    }

    [Fact]
    public void WriteWithColorHandlesEmptyOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.WriteWithColor("", ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Empty(output);
    }

    [Fact]
    public void WriteLineWithColorHandlesEmptyOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        
        // Act
        ConsoleHelper.WriteLineWithColor("", ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Equal(Environment.NewLine, output);
    }

    [Fact]
    public void WriteWithColorSupportsBothColors()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test message";
        
        // Act
        ConsoleHelper.WriteWithColor(testMessage, ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        // Should contain both foreground and background color codes
        Assert.Matches(@"\eb\[[0-9;]+m", output);
    }

    [Fact]
    public void WriteLineWithColorSupportsBothColors()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test message";
        
        // Act
        ConsoleHelper.WriteLineWithColor(testMessage, ConsoleColor.Cyan, ConsoleColor.DarkRed);
        
        // Assert
        var output = consoleOutput.StandardOutput;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        // Should contain both foreground and background color codes
        Assert.Matches(@"\eb\[[0-9;]+m", output);
    }

    [Fact]
    public void ErrorWriteWithColorProducesOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test error message";
        
        // Act
        ConsoleHelper.ErrorWriteWithColor(testMessage, ConsoleColor.Red);
        
        // Assert
        var output = consoleOutput.StandardError;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        Assert.Contains("m", output); // ANSI escape sequence ends with 'm'
    }

    [Fact]
    public void ErrorWriteLineWithColorProducesOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

        // Arrange
        using var consoleOutput = ConsoleOutput.Capture();
        var testMessage = "Test error message";
        
        // Act
        ConsoleHelper.ErrorWriteLineWithColor(testMessage, ConsoleColor.DarkRed);
        
        // Assert
        var output = consoleOutput.StandardError;
        Assert.Contains(testMessage, output);
        Assert.Contains("\eb[", output); // Should contain ANSI escape sequence
        Assert.Contains("m", output); // ANSI escape sequence ends with 'm'
    }

    [Fact]
    public void ErrorWriteWithColorHandlesNullOutput()
    {
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

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
        if (ConsoleHelper.SupportsAnsiColors() == false)
        {
            return;
        }

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
