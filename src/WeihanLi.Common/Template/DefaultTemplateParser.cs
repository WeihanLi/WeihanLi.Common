// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text.RegularExpressions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateParser : ITemplateParser
{
    private const string VariableRegexExp = @"\{\{(?<Variable>[\w\$\s:\.\|]+)\}\}";
    private static readonly Regex VariableRegex = new(VariableRegexExp, RegexOptions.Compiled);
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
            
            var pipeIndex = variableName.IndexOf('|');
            if (pipeIndex >= 0)
            {
                // exact pipes
                var pipeInputs = variableName[(pipeIndex + 1)..]
                    .Split(['|'], StringSplitOptions.RemoveEmptyEntries);
                variableName = variableName[..pipeIndex].Trim();
                pipes = pipeInputs.Select(p =>
                {
                    var pipeName = p.Trim();
                    var arguments = Array.Empty<string>();
                    var sep = pipeName.IndexOf(':');
                    if (sep >= 0)
                    {
                        pipeName = pipeName[..sep].Trim();
                        if (sep + 1 < pipeName.Length)
                        {
                            var argumentsText = pipeName[(sep + 1)..].Trim();
                            arguments =
#if NET
                            argumentsText.Split([':'],StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
#else
                                argumentsText.Split([':'], StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x=> x.Trim()).ToArray();
#endif                      
                        }
                    }
                    return new TemplatePipeInput()
                    {
                        PipeName = pipeName,
                        Arguments = arguments
                    };
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
