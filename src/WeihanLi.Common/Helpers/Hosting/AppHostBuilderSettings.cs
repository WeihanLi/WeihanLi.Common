// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;

namespace WeihanLi.Common.Helpers.Hosting;

public sealed class AppHostBuilderSettings
{
    public ConfigurationManager? Configuration { get; set; }
}
