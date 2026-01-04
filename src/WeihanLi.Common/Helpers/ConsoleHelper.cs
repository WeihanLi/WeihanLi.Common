// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public static class ConsoleHelper
{
    public static void InvokeWithConsoleColor(Action action, ConsoleColor? foregroundColor,
        ConsoleColor? backgroundColor = null)
    {
        Guard.NotNull(action);
        var originalForegroundColor = Console.ForegroundColor;
        var originalBackgroundColor = Console.BackgroundColor;
        try
        {
            if (foregroundColor.HasValue)
            {
                Console.ForegroundColor = foregroundColor.Value;
            }

            if (backgroundColor.HasValue)
            {
                Console.BackgroundColor = backgroundColor.Value;
            }

            action();
        }
        finally
        {
            Console.ForegroundColor = originalForegroundColor;
            Console.BackgroundColor = originalBackgroundColor;
        }
    }

    public static async Task InvokeWithConsoleColor(Func<Task> action, ConsoleColor? foregroundColor,
        ConsoleColor? backgroundColor = null)
    {
        Guard.NotNull(action);
        var originalForegroundColor = Console.ForegroundColor;
        var originalBackgroundColor = Console.BackgroundColor;
        try
        {
            if (foregroundColor.HasValue)
            {
                Console.ForegroundColor = foregroundColor.Value;
            }

            if (backgroundColor.HasValue)
            {
                Console.BackgroundColor = backgroundColor.Value;
            }

            await action();
        }
        finally
        {
            Console.ForegroundColor = originalForegroundColor;
            Console.BackgroundColor = originalBackgroundColor;
        }
    }

    public static void WriteWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (!string.IsNullOrEmpty(output))
        {
            var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
            Console.Write(coloredOutput);
        }
    }

    public static void WriteLineWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(output))
            Console.WriteLine();
        else
        {
            var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
            Console.WriteLine(coloredOutput);
        }
    }

    public static void ErrorWriteWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (!string.IsNullOrEmpty(output))
        {
            var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
            Console.Error.Write(coloredOutput);
        }
    }

    public static void ErrorWriteLineWithColor(string? error, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(error))
            Console.Error.WriteLine();
        else
        {
            var coloredOutput = ApplyAnsiColor(error!, foregroundColor, backgroundColor);
            Console.Error.WriteLine(coloredOutput);
        }
    }

    public static string? GetInput(string? inputPrompt = null, bool insertNewLine = true)
    {
        var input = ReadLineWithPrompt(inputPrompt);
        if (insertNewLine)
        {
            Console.WriteLine();
        }
        return input;
    }

    public static async Task HandleInputLoopAsync(Func<string, Task<bool>> handler,
        string? inputPrompt = null,
        string exitInput = "q",
        bool insertNewLine = true)
    {
        Guard.NotNull(handler);
        var input = GetInput(inputPrompt, insertNewLine);
        while (input != exitInput)
        {
            var result = await handler(input ?? string.Empty);
            if (!result) break;

            input = GetInput(inputPrompt);
        }
    }

    public static async Task HandleInputLoopAsync(Func<string, Task> handler,
        string? inputPrompt = null,
        string exitInput = "q",
        bool insertNewLine = true)
    {
        Guard.NotNull(handler);
        await HandleInputLoopAsync(async input =>
        {
            await handler.Invoke(input);
            return true;
        }, inputPrompt, exitInput, insertNewLine);
    }

    public static void HandleInputLoop(Func<string, bool> handler,
        string? inputPrompt = null,
        string exitInput = "q",
        bool insertNewLine = true)
    {
        Guard.NotNull(handler);
        var input = GetInput(inputPrompt, insertNewLine);
        while (input != exitInput)
        {
            var result = handler(input ?? string.Empty);
            if (!result) break;

            input = GetInput(inputPrompt, insertNewLine);
        }
    }

    public static void HandleInputLoop(Action<string> handler,
        string? inputPrompt = null,
        string exitInput = "q",
        bool insertNewLine = true)
    {
        Guard.NotNull(handler);
        HandleInputLoop(input =>
        {
            handler.Invoke(input);
            return true;
        }, inputPrompt, exitInput, insertNewLine);
    }

    public static string? ReadLineWithPrompt(string? prompt = "Press Enter to continue")
    {
        if (prompt is not null) Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    public static ConsoleKeyInfo ReadKeyWithPrompt(string? prompt = "Press any key to continue")
    {
        if (prompt is not null) Console.WriteLine(prompt);
        return Console.ReadKey();
    }

    public static void WriteLineIf(string? output, bool condition)
    {
        if (condition) Console.WriteLine(output);
    }

    public static void WriteIf(string? output, bool condition)
    {
        if (condition) Console.Write(output);
    }

    public static void ErrorWriteLineIf(string? output, bool condition)
    {
        if (condition) Console.Error.WriteLine(output);
    }

    public static void ErrorWriteIf(string? output, bool condition)
    {
        if (condition) Console.Error.Write(output);
    }

    public static CommandResult PrintOutputToConsole(this CommandResult commandResult)
    {
        Guard.NotNull(commandResult);

        Console.WriteLine(commandResult.StandardOut);

        if (!string.IsNullOrEmpty(commandResult.StandardError))
        {
            Console.WriteLine();
            ErrorWriteLineWithColor(commandResult.StandardError, ConsoleColor.DarkRed);
        }

        return commandResult;
    }

    public static bool HasStandardInput()
    {
        return Console.IsInputRedirected && Console.In.Peek() != -1;
    }
    
    public static bool TryGetStandardInput([MaybeNullWhen(false)]out string input)
    {
        if (HasStandardInput())
        {
            input = Console.In.ReadToEnd();
            return true;
        }
        
        input = null;
        return false;
    }

    private static string ApplyAnsiColor(string text, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
    {
        if (!foregroundColor.HasValue && !backgroundColor.HasValue)
            return text;

        var codes = new List<int>();
        
        if (foregroundColor.HasValue)
        {
            codes.Add(GetAnsiColorCode(foregroundColor.Value, isForeground: true));
        }
        
        if (backgroundColor.HasValue)
        {
            codes.Add(GetAnsiColorCode(backgroundColor.Value, isForeground: false));
        }

        var ansiStart = $"\x1b[{string.Join(";", codes)}m";
        var ansiReset = "\x1b[0m";
        
        return $"{ansiStart}{text}{ansiReset}";
    }

    private static int GetAnsiColorCode(ConsoleColor color, bool isForeground)
    {
        var baseCode = isForeground ? 30 : 40;
        
        return color switch
        {
            ConsoleColor.Black => baseCode + 0,
            ConsoleColor.DarkRed => baseCode + 1,
            ConsoleColor.DarkGreen => baseCode + 2,
            ConsoleColor.DarkYellow => baseCode + 3,
            ConsoleColor.DarkBlue => baseCode + 4,
            ConsoleColor.DarkMagenta => baseCode + 5,
            ConsoleColor.DarkCyan => baseCode + 6,
            ConsoleColor.Gray => baseCode + 7,
            ConsoleColor.DarkGray => baseCode + 60, // Bright black
            ConsoleColor.Red => baseCode + 61,      // Bright red
            ConsoleColor.Green => baseCode + 62,    // Bright green
            ConsoleColor.Yellow => baseCode + 63,   // Bright yellow
            ConsoleColor.Blue => baseCode + 64,     // Bright blue
            ConsoleColor.Magenta => baseCode + 65,  // Bright magenta
            ConsoleColor.Cyan => baseCode + 66,     // Bright cyan
            ConsoleColor.White => baseCode + 67,    // Bright white
            _ => baseCode + 7 // Default to gray
        };
    }
}
