// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

internal sealed class DefaultRenderMiddleware : IRenderMiddleware
{
    public async Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        await next(context);
    }
}
