// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Template;
public interface ITemplateRenderer
{
    [RequiresDynamicCode("Expression compilation requires dynamic code generation.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<string> RenderAsync(TemplateRenderContext template, object? globals);
}
