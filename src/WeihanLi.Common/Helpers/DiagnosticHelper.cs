// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;

namespace WeihanLi.Common.Helpers;

public static class DiagnosticHelper
{
    public const string ActivitySourceName = "WeihanLi.Common";
    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, 
        typeof(DiagnosticHelper).Assembly.GetName().Version?.ToString(3));
}
