// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

internal sealed class EnvRenderMiddleware : IRenderMiddleware
{
    private const string Prefix = "$env";
    public Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        foreach (var pair in context.Inputs
                     .Where(x => x.Key.Prefix is Prefix
                         && x.Value is null)
                 )
        {
            var variable = pair.Key.VariableName;
            context.Inputs[pair.Key] = Environment.GetEnvironmentVariable(variable);
        }
        return next(context);
    }
}
