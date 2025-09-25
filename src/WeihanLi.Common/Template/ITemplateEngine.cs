// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Template;

public interface ITemplateEngine
{
    [RequiresDynamicCode("Expression compilation requires dynamic code generation.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<string> RenderAsync(string text, object? globals = null);
}
