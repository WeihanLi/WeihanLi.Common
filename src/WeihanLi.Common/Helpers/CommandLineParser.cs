// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text;

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
    
    public static bool GetBoolArgumentValue(string[] args, string argumentName, bool defaultValue = default)
    {
        var value = ArgumentInternal(args, argumentName);
        return value switch
        {
            null => defaultValue,
            "" or "1" => true,
            "0" => false,
            _ => bool.Parse(value)
        };
    }
    
    public static string? GetArgumentValue(string[] args, string argumentName, string? defaultValue = default)
    {
        return ArgumentInternal(args, argumentName) ?? defaultValue;
    }

    private static string? ArgumentInternal(string[] args, string argumentName)
    {
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
