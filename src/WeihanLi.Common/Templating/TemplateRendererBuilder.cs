// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Templating;

internal sealed class TemplateRendererBuilder : ITemplateRendererBuilder
{
    private readonly IAsyncPipelineBuilder<TemplateRenderContext> _pipelineBuilder =
        PipelineBuilder.CreateAsync<TemplateRenderContext>();
    private Action<TemplateOptions>? _optionsAction;

    public ITemplateRendererBuilder Use(Func<TemplateRenderContext, Func<TemplateRenderContext, Task>, Task> middleware)
    {
        _pipelineBuilder.Use(middleware);
        return this;
    }

    public ITemplateRendererBuilder ConfigureOptions(Action<TemplateOptions> configureOptionsAction)
    {
        Guard.NotNull(configureOptionsAction);
        _optionsAction = configureOptionsAction;
        return this;
    }

    public ITemplateRenderer Build()
    {
        var options = new TemplateOptions();
        _optionsAction?.Invoke(options);
        return new DefaultTemplateRenderer(_pipelineBuilder.Build());
    }
}
