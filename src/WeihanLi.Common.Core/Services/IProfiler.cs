// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;

namespace WeihanLi.Common.Services;

public interface IProfiler
{
    /// <summary>Starts, or resumes, measuring elapsed time for an interval.</summary>
    void Start();

    /// <summary>Stops measuring elapsed time for an interval.</summary>
    void Stop();

    /// <summary>
    /// Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
    /// </summary>
    void Restart();

    /// <summary>
    /// Gets the total elapsed time measured by the current instance, in milliseconds.
    /// </summary>
    TimeSpan Elapsed { get; }
}

public sealed class StopwatchProfiler : IProfiler
{
    private readonly Stopwatch _stopwatch = new();

    public void Start()
    {
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
    }

    public void Restart()
    {
        _stopwatch.Restart();
    }

    public TimeSpan Elapsed => _stopwatch.Elapsed;
}
