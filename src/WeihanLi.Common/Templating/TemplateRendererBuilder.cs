// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Templating;

internal sealed class TemplateEngineBuilder : ITemplateEngineBuilder
{
    private readonly IAsyncPipelineBuilder<TemplateRenderContext> _pipelineBuilder =
        PipelineBuilder.CreateAsync<TemplateRenderContext>();
    private Action<TemplateEngineOptions>? _optionsConfigure;
    
    public ITemplateRendererBuilder UseRenderMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware: class, IRenderMiddleware
    {
        _pipelineBuilder.UseMiddleware(Guard.NotNull(middleware));
        return this;
    }

    public ITemplateRendererBuilder ConfigureOptions(Action<TemplateEngineOptions> optionsConfigure)
    {
        _optionsConfigure = Guard.NotNull(optionsConfigure);
        return this;
    }

    public ITemplateParser BuildParser() => new DefaultTemplateParser();

    public ITemplateRenderer BuildRenderer()
    {
        var options = new TemplateEngineOptions();
        _optionsConfigure?.Invoke(options);
        _pipelineBuilder
            .UseMiddleware(new DefaultRenderMiddleware())
            .UseMiddleware(new EnvRenderMiddleware())
            .UseMiddleware(new ConfigurationRenderMiddleware(options.Configuration))
            ;
        return new DefaultTemplateRenderer(_pipelineBuilder.Build());
    }
}
