// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

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

    public async Task<string> RenderAsync(string text, object parameters)
    {
        var context = await _templateParser.ParseAsync(text);
        var result = await _templateRenderer.RenderAsync(context, parameters);
        return result;
    }

    public static TemplateEngine CreateDefault() =>
        new TemplateEngine(new DefaultTemplateParser(), new DefaultTemplateRenderer(context =>
        {
            if (context.Text.IsNullOrWhiteSpace())
            {
                return Task.CompletedTask;
            }
            context.RenderedText = context.Text;
            foreach (var parameter in context.Parameters)
            {
                context.RenderedText = context.RenderedText.Replace($"{{{{{parameter.Key}}}}}", parameter.Value?.ToString());
            }
            return Task.CompletedTask;
        }));
}
