// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Templating;

internal sealed class DefaultRenderMiddleware : IRenderMiddleware
{
    public Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        if (context.Text.IsNotNullOrWhiteSpace())
        {
            context.RenderedText = context.Text;
            foreach (var parameter in context.Parameters)
            {
                context.RenderedText = context.RenderedText.Replace($"{{{{{parameter.Key}}}}}", parameter.Value?.ToString());
            }
        }
        
        return next(context);
    }
}
