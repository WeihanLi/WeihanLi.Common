// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Template;

internal sealed class TemplateEngineBuilder : ITemplateEngineBuilder
{
    private readonly IAsyncPipelineBuilder<TemplateRenderContext> _pipelineBuilder =
        PipelineBuilder.CreateAsync<TemplateRenderContext>();
    private Action<TemplateEngineOptions>? _optionsConfigure;
    private readonly Dictionary<string, ITemplatePipe> _pipes = new();

    public ITemplateRendererBuilder UseTemplatePipe<TPipe>(TPipe pipe) where TPipe : class, ITemplatePipe
    {
        _pipes[pipe.Name] = pipe;
        return this;
    }

    public ITemplateRendererBuilder UseRenderMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : class, IRenderMiddleware
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
        var options = new TemplateEngineOptions
        {
            Pipes = _pipes
        };
        _optionsConfigure?.Invoke(options);
        
        _pipelineBuilder
            .UseMiddleware(new DefaultRenderMiddleware(options.Pipes))
            .UseMiddleware(new EnvRenderMiddleware())
            .UseMiddleware(new ConfigurationRenderMiddleware(options.Configuration))
            ;
        return new DefaultTemplateRenderer(_pipelineBuilder.Build());
    }
}
