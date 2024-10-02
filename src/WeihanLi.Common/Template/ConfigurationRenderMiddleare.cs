// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;

namespace WeihanLi.Common.Template;

internal sealed class ConfigurationRenderMiddleware(IConfiguration? configuration = null)
    : IRenderMiddleware
{
    private const string Prefix = "$config ";
    public Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        if (configuration != null)
        {
            foreach (var variable in context.Inputs.Where(x => x.StartsWith(Prefix) && !context.Parameters.ContainsKey(x)))
            {
                context.Parameters[variable] = configuration[variable[Prefix.Length..]];
            }
        }

        return next(context);
    }
}
