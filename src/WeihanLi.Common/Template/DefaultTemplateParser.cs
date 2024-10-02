// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text.RegularExpressions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateParser : ITemplateParser
{
    private const string VariableRegexExp = @"\{\{(?<Variable>[\w\$\s:\.]+)\}\}";
    private static readonly Regex VariableRegex = new(VariableRegexExp, RegexOptions.Compiled);
    public Task<TemplateRenderContext> ParseAsync(string text)
    {
        var inputs = new HashSet<string>();
        var match = VariableRegex.Match(text);
        while (match.Success)
        {
            var variable = match.Groups["Variable"].Value;
            inputs.Add(variable);
            match = match.NextMatch();
        }
        var context = new TemplateRenderContext(text, inputs);
        return Task.FromResult(context);
    }
}
