// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateRenderer(Func<TemplateRenderContext, Task> renderFunc) : ITemplateRenderer
{
    public async Task<string> RenderAsync(TemplateRenderContext context, object? globals)
    {
        if (context.Text.IsNullOrWhiteSpace() || context.Variables.IsNullOrEmpty())
            return context.Text;

        context.Parameters = globals.ParseParamDictionary();
        await renderFunc.Invoke(context).ConfigureAwait(false);
        foreach (var parameter in context.Parameters)
        {
            context.RenderedText = context.RenderedText.Replace(
                    $"{{{{{parameter.Key}}}}}", parameter.Value?.ToString()
                    );
        }
        return context.RenderedText;
    }
}
