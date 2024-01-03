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
            InvokeWithConsoleColor(() => Console.Write(output), foregroundColor, backgroundColor);
    }

    public static void WriteLineWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(output))
            Console.WriteLine();
        else
            InvokeWithConsoleColor(() => Console.WriteLine(output), foregroundColor, backgroundColor);
    }

    public static void ErrorWriteWithColor(string? output, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (!string.IsNullOrEmpty(output))
            InvokeWithConsoleColor(() => Console.Error.Write(output), foregroundColor, backgroundColor);
    }

    public static void ErrorWriteLineWithColor(string error, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
    {
        if (string.IsNullOrEmpty(error))
            Console.Error.WriteLine();
        else
            InvokeWithConsoleColor(() => Console.Error.WriteLine(error), foregroundColor, backgroundColor);
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
}
