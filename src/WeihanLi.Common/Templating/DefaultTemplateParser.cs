// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text.RegularExpressions;

namespace WeihanLi.Common.Templating;

public sealed class DefaultTemplateParser : ITemplateParser
{
    private const string VariableRegexExp = @"\{\{(?<Variable>.+)\}\}";
    private static readonly Regex VariableRegex = new(VariableRegexExp, RegexOptions.Compiled);
    public Task<TemplateRenderContext> ParseAsync(string text)
    {
        var variables = new HashSet<string>();
        var match = VariableRegex.Match(text); ;
        while (match.Success)
        {
            var variable = match.Groups["Variable"].Value;
            if (!string.IsNullOrEmpty(variable))
            {
                variables.Add(variable);
            }
            match = match.NextMatch();
        }
        var context = new TemplateRenderContext(text, variables);
        return Task.FromResult(context);
    }
}
