// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

internal sealed class DefaultRenderMiddleware(Dictionary<string, ITemplatePipe> pipes) : IRenderMiddleware
{
    public async Task InvokeAsync(TemplateRenderContext context, Func<TemplateRenderContext, Task> next)
    {
        await next(context);
        foreach (var input in context.Inputs.Keys)
        {
            var value = context.Inputs[input];
            if (input.Pipes is { Length: > 0 })
            {
                foreach (var pipeInput in input.Pipes)
                {
                    if (pipes.TryGetValue(pipeInput.PipeName, out var pipe))
                    {
                        value = pipe.Convert(value, pipeInput.Arguments);
                    }
                }
            }
            // replace input with value
            context.RenderedText = context.RenderedText.Replace(input.Input, value as string ?? value?.ToString());
        }
    }
}
