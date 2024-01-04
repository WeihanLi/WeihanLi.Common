// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Abstractions;

namespace WeihanLi.Common.Template;
public sealed class TemplateRenderContext : IProperties
{
    public TemplateRenderContext(string text, HashSet<string> variables)
    {
        Text = text;
        Variables = variables;
        RenderedText = string.Empty;
    }

    public string Text { get; }
    public HashSet<string> Variables { get; }
    public string RenderedText { get; set; }
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
}
