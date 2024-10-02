// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;

namespace WeihanLi.Common.Template;

internal sealed class ConfigurationRenderMiddleware(IConfiguration? configuration = null)
    : IRenderMiddleware
{
    private const string Prefix = "$config";
    public Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        if (configuration is not null)
        {
            foreach (var pair in context.Inputs
                         .Where(x => x.Key.Prefix is Prefix
                                     && x.Value is null)
                    )
            {
                context.Inputs[pair.Key] = configuration[pair.Key.VariableName];
            }
        }

        return next(context);
    }
}
