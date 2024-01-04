// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;

namespace WeihanLi.Common.Template;
public sealed class TemplateEngineOptions
{
    public IConfiguration? Configuration { get; set; }
}
