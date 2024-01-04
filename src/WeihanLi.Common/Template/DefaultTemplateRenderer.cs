// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Template;

internal sealed class DefaultTemplateRenderer(Func<TemplateRenderContext, Task> renderFunc) : ITemplateRenderer
{
    public async Task<string> RenderAsync(TemplateRenderContext context, object? globals)
    {
        context.Parameters = globals.ParseParamDictionary();
        await renderFunc.Invoke(context).ConfigureAwait(false);
        return context.RenderedText;
    }
}
