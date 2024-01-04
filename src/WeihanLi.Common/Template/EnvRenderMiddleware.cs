// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

internal sealed class EnvRenderMiddleware : IRenderMiddleware
{
    private const string Prefix = "$env ";
    public Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        foreach (var variable in context.Variables.Where(x => x.StartsWith(Prefix) && !context.Parameters.ContainsKey(x)))
        {
            context.Parameters[variable] = Environment.GetEnvironmentVariable(variable[Prefix.Length..]);
        }
        return next(context);
    }
}
