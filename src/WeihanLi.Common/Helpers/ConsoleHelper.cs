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
}
