// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Templating;

internal sealed class DefaultTemplateRenderer : ITemplateRenderer
{
    private readonly Func<TemplateRenderContext, Task> _renderFunc;

    public DefaultTemplateRenderer(Func<TemplateRenderContext, Task> renderFunc)
    {
        _renderFunc = renderFunc;
    }

    public async Task<string> RenderAsync(TemplateRenderContext context, object? globals)
    {
        context.Parameters = globals.ParseParamDictionary();
        await _renderFunc.Invoke(context);
        return context.RenderedText;
    }
}
