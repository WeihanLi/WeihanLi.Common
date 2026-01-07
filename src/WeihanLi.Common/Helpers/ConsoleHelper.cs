// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Runtime.InteropServices;

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
            if (SupportsAnsiColors())
            {
                var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
                Console.Write(coloredOutput);
            }
            else
            {
                InvokeWithConsoleColor(() => Console.Write(output), foregroundColor, backgroundColor);
            }
        }
    }

    public static void WriteLineWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(output))
            Console.WriteLine();
        else
        {
            if (SupportsAnsiColors())
            {
                var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
                Console.WriteLine(coloredOutput);
            }
            else
            {
                InvokeWithConsoleColor(() => Console.WriteLine(output), foregroundColor, backgroundColor);
            }
        }
    }

    public static void ErrorWriteWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (!string.IsNullOrEmpty(output))
        {
            if (SupportsAnsiColors())
            {
                var coloredOutput = ApplyAnsiColor(output!, foregroundColor, backgroundColor);
                Console.Error.Write(coloredOutput);
            }
            else
            {
                InvokeWithConsoleColor(() => Console.Error.Write(output), foregroundColor, backgroundColor);
            }
        }
    }

    public static void ErrorWriteLineWithColor(string? error, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(error))
            Console.Error.WriteLine();
        else
        {
            if (SupportsAnsiColors())
            {
                var coloredOutput = ApplyAnsiColor(error!, foregroundColor, backgroundColor);
                Console.Error.WriteLine(coloredOutput);
            }
            else
            {
                InvokeWithConsoleColor(() => Console.Error.WriteLine(error), foregroundColor, backgroundColor);
            }
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

    /// <summary>
    /// Determines whether the console supports ANSI escape sequences for color output.
    /// </summary>
    /// <returns>true if ANSI colors are supported; otherwise, false.</returns>
    public static bool SupportsAnsiColors()
    {
        // Check for explicit environment variable to disable ANSI colors
        var noColor = Environment.GetEnvironmentVariable("NO_COLOR");
        if (!string.IsNullOrEmpty(noColor))
            return false;

        // On Windows, check for Virtual Terminal Processing support
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows Terminal and newer Windows versions support ANSI
            var wtSession = Environment.GetEnvironmentVariable("WT_SESSION");
            if (!string.IsNullOrEmpty(wtSession))
                return true;

            // Check Windows version - Windows 10 build 10586 and later support VT
            try
            {
                var version = Environment.OSVersion.Version;
                if (version.Major >= 10 && version.Build >= 10586)
                    return true;
            }
            catch (PlatformNotSupportedException)
            {
                // Platform doesn't support version detection
                return false;
            }
            catch (InvalidOperationException)
            {
                // Unable to determine OS version
                return false;
            }

            return false;
        }

        // On Unix-like systems, check the TERM environment variable
        var term = Environment.GetEnvironmentVariable("TERM");
        
        // Most modern terminals support ANSI colors
        // Only explicitly reject "dumb" terminals
        if (term == "dumb")
            return false;

        return true;
    }

    private static string ApplyAnsiColor(string text, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (!foregroundColor.HasValue && !backgroundColor.HasValue)
            return text;

        var hasForeground = foregroundColor.HasValue;
        var hasBackground = backgroundColor.HasValue;
        
        var fgCode = 0;
        var bgCode = 0;
        if (hasForeground)
        {
            fgCode = GetAnsiColorCode(foregroundColor!.Value, isForeground: true);
        }
        if (hasBackground)
        {
            bgCode = GetAnsiColorCode(backgroundColor!.Value, isForeground: false);
        }

        var ansiStart = (hasForeground, hasBackground) switch
        {
            (true, true) => $"\e[{fgCode};{bgCode}m",
            (true, false) => $"\e[{fgCode}m",
            _ => $"\e[{bgCode}m"
        };
        var ansiReset = "\e[0m";
        
        return $"{ansiStart}{text}{ansiReset}";
    }

    private static int GetAnsiColorCode(ConsoleColor color, bool isForeground)
    {
        return color switch
        {
            // Standard colors (30-37 for foreground, 40-47 for background)
            ConsoleColor.Black => isForeground ? 30 : 40,
            ConsoleColor.DarkRed => isForeground ? 31 : 41,
            ConsoleColor.DarkGreen => isForeground ? 32 : 42,
            ConsoleColor.DarkYellow => isForeground ? 33 : 43,
            ConsoleColor.DarkBlue => isForeground ? 34 : 44,
            ConsoleColor.DarkMagenta => isForeground ? 35 : 45,
            ConsoleColor.DarkCyan => isForeground ? 36 : 46,
            ConsoleColor.Gray => isForeground ? 37 : 47,
            // Bright colors (90-97 for foreground, 100-107 for background)
            ConsoleColor.DarkGray => isForeground ? 90 : 100,
            ConsoleColor.Red => isForeground ? 91 : 101,
            ConsoleColor.Green => isForeground ? 92 : 102,
            ConsoleColor.Yellow => isForeground ? 93 : 103,
            ConsoleColor.Blue => isForeground ? 94 : 104,
            ConsoleColor.Magenta => isForeground ? 95 : 105,
            ConsoleColor.Cyan => isForeground ? 96 : 106,
            ConsoleColor.White => isForeground ? 97 : 107,
            _ => isForeground ? 37 : 47 // Default to gray
        };
    }
}
