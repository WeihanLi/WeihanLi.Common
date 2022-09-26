// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;

namespace WeihanLi.Common.Templating;
public sealed class TemplateEngine : ITemplateRenderer, ITemplateParser
{
    private readonly ITemplateParser _templateParser;
    private readonly ITemplateRenderer _templateRenderer;
    public TemplateEngine(ITemplateParser templateParser, ITemplateRenderer templateRenderer)
    {
        _templateParser = templateParser;
        _templateRenderer = templateRenderer;
    }
    public Task<TemplateRenderContext> ParseAsync(string text)
    {
        return _templateParser.ParseAsync(text);
    }
    public async Task<string> RenderAsync(TemplateRenderContext context, object globals)
    {
        return await _templateRenderer.RenderAsync(context, globals);
    }

    public static TemplateEngine CreateDefault() =>
        new TemplateEngine(new DefaultTemplateParser(), new DefaultTemplateRenderer(null));
}
