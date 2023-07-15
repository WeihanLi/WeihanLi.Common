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
    
    public static string? GetInput(string? prompt = null, bool insertNewLine = true)
    {
        Console.WriteLine(prompt ?? "Enter your input here");
        var input = Console.ReadLine();
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

            input = GetInput(inputPrompt);
        }
    }
    
    public static void HandleInputLoop(Action<string> handler, 
        string? inputPrompt = null, 
        string exitInput = "q", 
        bool insertNewLine = true)
    {
        Guard.NotNull(handler);
        HandleInputLoop( input =>
        {
            handler.Invoke(input);
            return true;
        }, inputPrompt, exitInput, insertNewLine);
    }
}
