// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Abstractions;

namespace WeihanLi.Common.Template;
public sealed class TemplateRenderContext(string text, HashSet<string> variables) : IProperties
{
    public string Text { get; } = text;
    public HashSet<string> Variables { get; } = variables;
    public string RenderedText { get; set; } = text;
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
}
