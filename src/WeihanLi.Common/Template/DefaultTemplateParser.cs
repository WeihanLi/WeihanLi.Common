// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text.RegularExpressions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateParser : ITemplateParser
{
    private const string VariableGroupRegexExp = @"\{\{(?<Variable>[\w\$\s:\.]+)(?<Pipe>|[^\{\}]*)\}\}";
    private static readonly Regex VariableRegex = new(VariableGroupRegexExp, RegexOptions.Compiled);
    public Task<TemplateRenderContext> ParseAsync(string text)
    {
        List<TemplateInput> inputs = [];
        var match = VariableRegex.Match(text);
        while (match.Success)
        {
            var pipes = Array.Empty<TemplatePipeInput>();
            var variableInput = match.Groups["Variable"].Value;
            var variableName = variableInput.Trim();
            string? prefix = null;

            var prefixIndex = variableName.IndexOf('$'); // prefix start
            if (prefixIndex >= 0)
            {
                var nameIndex = variableName.IndexOf(' ', prefixIndex); // name start
                prefix = variableName[..nameIndex].Trim();
                variableName = variableName[nameIndex..].Trim();
            }

            var pipeValue = match.Groups["Pipe"]?.Value.Trim();
            if (!string.IsNullOrEmpty(pipeValue))
            {
                var pipeIndex = pipeValue!.IndexOf('|');
                if (pipeIndex < 0)
                {
                    match = match.NextMatch();
                    continue;
                }

                // exact pipes
                pipeValue = pipeValue[pipeIndex..].Trim();
                var pipeInputs = pipeValue.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
                pipes = pipeInputs.Select(p =>
                    {
                        var pipeName = p.Trim();
                        var arguments = Array.Empty<string>();
                        var sep = pipeName.IndexOf(':');
                        if (sep >= 0)
                        {
                            if (sep + 1 < pipeName.Length)
                            {
                                var argumentsText = pipeName[(sep + 1)..].Trim();
                                arguments =
#if NET
                                    argumentsText.Split([':'],
                                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#else
                                argumentsText.Split([':'], StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim()).ToArray();
#endif
                            }

                            pipeName = pipeName[..sep].Trim();
                        }

                        return new TemplatePipeInput() { PipeName = pipeName, Arguments = arguments };
                    }).ToArray();
            }

            var input = new TemplateInput
            {
                Input = match.Value,
                Prefix = prefix,
                VariableName = variableName,
                Pipes = pipes
            };
            inputs.Add(input);

            match = match.NextMatch();
        }
        var context = new TemplateRenderContext(text, inputs);
        return Task.FromResult(context);
    }
}
