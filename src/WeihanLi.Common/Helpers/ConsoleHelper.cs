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

    public static ConsoleKeyInfo ReadKeyWithPrompt(string? prompt = "Press Enter to continue")
    {
        if (prompt is not null) Console.WriteLine(prompt);
        return Console.ReadKey();
    }

    public static void WriteLineIf(string output, bool condition)
    {
        if (condition) Console.WriteLine(output);
    }
    
    public static void WriteIf(string output, bool condition)
    {
        if (condition) Console.Write(output);
    }
}
