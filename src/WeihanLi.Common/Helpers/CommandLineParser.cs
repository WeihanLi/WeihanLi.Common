﻿// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class CommandLineParser
{
    public static IEnumerable<string> ParseLine(string line, LineParseOptions? options = null)
    {
        if (string.IsNullOrEmpty(line))
        {
            yield break;
        }

        options ??= new();
        var tokenBuilder = new StringBuilder();

        var inToken = false;
        var inQuotes = false;

        // Iterate through every character in the line
        for (var i = 0; i < line.Length; i++)
        {
            var character = line[i];

            // If we are not currently inside a column
            if (!inToken)
            {
                // If the current character is a quote then the token value is contained within
                // quotes, otherwise append the next character
                inToken = true;
                if (character == options.Quote)
                {
                    inQuotes = true;
                    continue;
                }
            }

            // If we are in between quotes
            if (inQuotes)
            {
                if (i + 1 == line.Length)
                {
                    break;
                }

                if (character == options.Quote && line[i + 1] == options.Separator) // quotes end
                {
                    inQuotes = false;
                    inToken = false;
                    i++; //skip next
                }
                else if (character == options.Quote && line[i + 1] == options.Quote) // quotes
                {
                    i++; //skip next
                }
                else if (character == options.Quote)
                {
                    throw new ArgumentException($"unable to escape {line}");
                }
            }
            else if (character == options.Separator)
            {
                inToken = false;
            }

            // If we are no longer in the token clear the builder and add the columns to the list
            if (!inToken)
            {
                if (!options.RemoveEmptyToken || tokenBuilder.Length > 0)
                {
                    yield return tokenBuilder.ToString();
                    tokenBuilder.Clear();
                }
            }
            else
            {
                tokenBuilder.Append(character);
            }
        }

        if (!options.RemoveEmptyToken || tokenBuilder.Length > 0)
        {
            yield return tokenBuilder.ToString();
            tokenBuilder.Clear();
        }
    }

    /// <summary>
    /// Get argument value from arguments
    /// </summary>
    /// <param name="optionName">argument name to get value</param>
    /// <param name="args">arguments</param>
    /// <param name="defaultValue">default argument value when not found</param>
    /// <returns>argument value</returns>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string optionName, string[]? args = null, string? defaultValue = default)
    {
        return GetValueInternal(args ?? Environment.GetCommandLineArgs(), optionName) ?? defaultValue;
    }

    /// <summary>
    /// Get argument value from arguments
    /// </summary>
    /// <param name="optionName">argument name to get value</param>
    /// <param name="defaultValue">default argument value when not found</param>
    /// <param name="args">arguments</param>
    /// <returns>argument value</returns>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string optionName, string? defaultValue, string[]? args = null)
    {
        return GetValueInternal(args ?? Environment.GetCommandLineArgs(), optionName) ?? defaultValue;
    }

    /// <summary>
    /// Get argument value from arguments
    /// </summary>
    /// <param name="args">arguments</param>
    /// <param name="defaultValue">default argument value when not found</param>
    /// <param name="optionName">argument name to get value</param>
    /// <returns>argument value</returns>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string[] args, string optionName, string? defaultValue = null)
    {
        return GetValueInternal(args, optionName) ?? defaultValue;
    }

    /// <summary>
    /// Get boolean argument value from arguments
    /// </summary>
    /// <param name="optionName">argument name to get value</param>
    /// <param name="args">arguments</param>
    /// <param name="defaultValue">default argument value when not found</param>    
    /// <returns>Boolean value</returns>
    public static bool BooleanVal(string optionName, string[]? args = null, bool defaultValue = default)
    {
        return GetValueInternal(args ?? Environment.GetCommandLineArgs(), optionName).ToBoolean(defaultValue);
    }

    /// <summary>
    /// Get boolean argument value from arguments
    /// </summary>    
    /// <param name="optionName">argument name to get value</param>
    /// <param name="defaultValue">default argument value when not found</param>
    /// <param name="args">arguments</param>
    /// <returns>Boolean value</returns>
    public static bool BooleanVal(string optionName, bool defaultValue, string[]? args = null)
    {
        return GetValueInternal(args ?? Environment.GetCommandLineArgs(), optionName).ToBoolean(defaultValue);
    }

    /// <summary>
    /// Get boolean argument value from arguments
    /// </summary>    
    /// <param name="args">arguments</param>
    /// <param name="optionName">argument name to get value</param>
    /// <param name="defaultValue">default argument value when not found</param>    
    /// <returns>Boolean value</returns>
    public static bool BooleanVal(string[] args, string optionName, bool defaultValue = default)
    {
        return GetValueInternal(args, optionName).ToBoolean(defaultValue);
    }

    private static string? GetValueInternal(string[] args, string argumentName)
    {
        Guard.NotNull(args);
        Guard.NotNullOrEmpty(argumentName);
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == $"--{argumentName}" || args[i] == $"-{argumentName}")
            {
                if (i + 1 == args.Length || args[i + 1].StartsWith('-'))
                    return string.Empty;

                return args[i + 1];
            }

            if (args[i].StartsWith($"-{argumentName}=", StringComparison.Ordinal)
                || args[i].StartsWith($"-{argumentName}:", StringComparison.Ordinal))
                return args[i][$"-{argumentName}=".Length..];

            if (args[i].StartsWith($"--{argumentName}=", StringComparison.Ordinal)
                || args[i].StartsWith($"--{argumentName}:", StringComparison.Ordinal))
                return args[i][$"--{argumentName}=".Length..];
        }

        return null;
    }
}

public sealed class LineParseOptions
{
    public char Separator { get; set; } = ' ';
    public char Quote { get; set; } = '\'';
    public bool RemoveEmptyToken { get; set; }
}
