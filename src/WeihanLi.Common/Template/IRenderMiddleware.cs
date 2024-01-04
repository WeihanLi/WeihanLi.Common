// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Template;

public interface IRenderMiddleware : IAsyncPipelineMiddleware<TemplateRenderContext>
{
}
