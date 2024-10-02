// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Common.Abstractions;

namespace WeihanLi.Common.Template;

public sealed class TemplateRenderContext(string text, IReadOnlyCollection<TemplateInput> inputs)
    : IProperties
{
    public string Text { get; } = text;
    public Dictionary<TemplateInput, object?> Inputs { get; } = 
        inputs.ToDictionary(x=> x, _ => (object?)null);
    public string RenderedText { get; set; } = text;
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
}

public sealed class TemplateInput : IEquatable<TemplateInput>
{
    public required string Input { get; init; }
    public required string? Prefix { get; init; }
    public required string VariableName { get; init; }
    public required TemplatePipeInput[] Pipes { get; init; }
    public bool Equals(TemplateInput other) => other.Input == Input;
    public override bool Equals(object? obj) => obj is TemplateInput input && Equals(input);
    public override int GetHashCode() => Input.GetHashCode();
}

[DebuggerDisplay("{PipeName:nq}")]
public sealed class TemplatePipeInput
{
    public required string PipeName { get; init; }
    public required string[]? Arguments { get; set; }
}
