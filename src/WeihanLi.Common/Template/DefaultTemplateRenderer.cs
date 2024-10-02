// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateRenderer(Func<TemplateRenderContext, Task> renderFunc)
    : ITemplateRenderer
{
    public async Task<string> RenderAsync(TemplateRenderContext context, object? globals)
    {
        if (context.Text.IsNullOrWhiteSpace() || context.Inputs.IsNullOrEmpty())
            return context.Text;
        
        var parameters = globals.ParseParamDictionary();
        if (parameters is { Count: > 0 })
        {
            foreach (var input in context.Inputs.Keys.Where(x => x.Prefix is null))
            {
                if (parameters.TryGetValue(input.VariableName, out var value))
                {
                    context.Inputs[input] = value;
                }
            }   
        }
        await renderFunc.Invoke(context).ConfigureAwait(false);
        return context.RenderedText;
    }
}
