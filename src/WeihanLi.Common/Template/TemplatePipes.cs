// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Globalization;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Template;

public abstract class TemplatePipeBase : ITemplatePipe
{
    protected virtual int? ParameterCount => 1;

    public abstract string Name { get; }
    public object? Convert(object? value, params ReadOnlySpan<string> args)
    {
        if (ParameterCount.HasValue && ParameterCount.Value != args.Length)
        {
            throw new InvalidOperationException($"The number of arguments {args.Length} must be equal to the parameter count({ParameterCount}).");
        }

        return ConvertInternal(value, args);
    }

    protected abstract object? ConvertInternal(object? value, params ReadOnlySpan<string> args);
}

public sealed class TextFormatTemplatePipe : TemplatePipeBase
{
    protected override object? ConvertInternal(object? value, params ReadOnlySpan<string> args)
        => FormatText(value, args[0]);

    public override string Name => "format";
    private string? FormatText(object? value, string format)
    {
        return value switch
        {
            null => null,
            IFormattable text => text.ToString(format, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}

public abstract class TextTransformTemplatePipe : TemplatePipeBase
{
    protected override int? ParameterCount => 0;

    protected override object? ConvertInternal(object? value, params ReadOnlySpan<string> args)
    {
        var str = value as string ?? value?.ToString();
        return str is null ? null : ConvertText(str);
    }

    protected abstract string? ConvertText(string value);
}

public sealed class UpperCaseTemplatePipe : TextTransformTemplatePipe
{
    public override string Name => "toUpper";
    protected override string ConvertText(string value)
    {
        return value.ToUpperInvariant();
    }
}

public sealed class LowerCaseTemplatePipe : TextTransformTemplatePipe
{
    public override string Name => "toLower";
    protected override string ConvertText(string value)
    {
        return value.ToLowerInvariant();
    }
}

public sealed class TitleCaseTemplatePipe : TextTransformTemplatePipe
{
    public override string Name => "toTitle";
    protected override string ConvertText(string value)
    {
        return value.ToTitleCase();
    }
}
