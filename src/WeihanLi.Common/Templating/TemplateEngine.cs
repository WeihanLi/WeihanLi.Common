// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Templating;
public sealed class TemplateEngine(ITemplateParser templateParser, ITemplateRenderer templateRenderer)
    : ITemplateEngine, ITemplateRenderer, ITemplateParser
{
    private readonly ITemplateParser _templateParser = templateParser;
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;

    public Task<TemplateRenderContext> ParseAsync(string text)
    {
        return _templateParser.ParseAsync(text);
    }
    public async Task<string> RenderAsync(TemplateRenderContext context, object? globals)
    {
        return await _templateRenderer.RenderAsync(context, globals);
    }

    public async Task<string> RenderAsync(string text, object? parameters = null)
    {
        var context = await _templateParser.ParseAsync(text);
        var result = await _templateRenderer.RenderAsync(context, parameters);
        return result;
    }

    public static TemplateEngine CreateDefault(Action<ITemplateEngineBuilder>? builderConfigure = null)
    {
        var builder = new TemplateEngineBuilder();
        builderConfigure?.Invoke(builder);
        return new TemplateEngine(builder.BuildParser(), builder.BuildRenderer());
    }
}
