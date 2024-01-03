// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Templating;

internal sealed class TemplateEngineBuilder : ITemplateEngineBuilder
{
    private readonly IAsyncPipelineBuilder<TemplateRenderContext> _pipelineBuilder =
        PipelineBuilder.CreateAsync<TemplateRenderContext>();
    private Action<TemplateEngineOptions>? _optionsAction;
    public ITemplateRendererBuilder UseRenderMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware: class, IRenderMiddleware
    {
        _pipelineBuilder.UseMiddleware(middleware);
        return this;
    }

    public ITemplateRendererBuilder ConfigureOptions(Action<TemplateEngineOptions> configureOptionsAction)
    {
        Guard.NotNull(configureOptionsAction);
        _optionsAction = configureOptionsAction;
        return this;
    }

    public ITemplateParser BuildParser()
    {
        return new DefaultTemplateParser();
    }
    
    public ITemplateRenderer BuildRenderer()
    {
        var options = new TemplateEngineOptions();
        _optionsAction?.Invoke(options);
        _pipelineBuilder
            .UseMiddleware(new EnvRenderMiddleware())
            .UseMiddleware(new ConfigurationRenderMiddleware(options.Configuration))
            .UseMiddleware(new DefaultRenderMiddleware())
            ;
        return new DefaultTemplateRenderer(_pipelineBuilder.Build());
    }
}
