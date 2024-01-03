// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Templating;

internal sealed class DefaultRenderMiddleware : IRenderMiddleware
{
    public async Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        if (context.Text.IsNullOrWhiteSpace()) return;
        
        await next(context);
        
        context.RenderedText = context.Text;
        foreach (var parameter in context.Parameters)
        {
            context.RenderedText =
                context.RenderedText.Replace($"{{{{{parameter.Key}}}}}", parameter.Value?.ToString());
        }
    }
}
