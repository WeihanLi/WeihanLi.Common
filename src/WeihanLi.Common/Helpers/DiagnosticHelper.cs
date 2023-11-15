// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace WeihanLi.Common.Helpers;

public static class DiagnosticHelper
{
    private const string DiagnosticSourceName = "WeihanLi.Common";
    public static readonly ActivitySource ActivitySource;
    public static readonly Meter Meter;
    static DiagnosticHelper()
    {
        var version = typeof(DiagnosticHelper).Assembly.GetName().Version?.ToString(3);
        ActivitySource = new(DiagnosticSourceName, version);
        Meter = new(DiagnosticSourceName, version);
    }
}
