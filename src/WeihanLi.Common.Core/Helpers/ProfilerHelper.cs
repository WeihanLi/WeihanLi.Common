// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Common.Services;

namespace WeihanLi.Common.Helpers;

public sealed class ProfilerStopper(IProfiler profiler, Action<TimeSpan> profileAction) : IDisposable
{
    private readonly IProfiler _profiler = Guard.NotNull(profiler);
    private readonly Action<TimeSpan> _profileAction = Guard.NotNull(profileAction);

    public void Dispose()
    {
        _profiler.Stop();
        _profileAction(_profiler.Elapsed);
    }
}

public sealed class StopwatchStopper(Stopwatch stopwatch, Action<TimeSpan> profileAction) : IDisposable
{
    private readonly Stopwatch _stopwatch = Guard.NotNull(stopwatch);
    private readonly Action<TimeSpan> _profileAction = Guard.NotNull(profileAction);

    public void Dispose()
    {
        _stopwatch.Stop();
        _profileAction(_stopwatch.Elapsed);
    }
}

public static class ProfilerHelper
{
    public static StopwatchStopper Profile(this Stopwatch watch, Action<TimeSpan> profilerAction)
    {
        Guard.NotNull(watch).Restart();
        return new StopwatchStopper(watch, profilerAction);
    }

    public static ProfilerStopper StartNew(this IProfiler profiler, Action<TimeSpan> profilerAction)
    {
        Guard.NotNull(profiler).Restart();
        return new ProfilerStopper(profiler, profilerAction);
    }

    public static readonly double TicksPerTimestamp = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    /// <summary>
    /// GetElapsedTime
    /// </summary>
    /// <param name="startTimestamp">startTimestamp, get by Stopwatch.GetTimestamp()</param>
    /// <returns>elapsed time</returns>
    public static TimeSpan GetElapsedTime(long startTimestamp) =>
#if NET
        Stopwatch.GetElapsedTime(startTimestamp)
#else
        GetElapsedTime(startTimestamp, Stopwatch.GetTimestamp())
#endif
        ;

    /// <summary>
    /// GetElapsedTime
    /// </summary>
    /// <param name="startTimestamp">startTimestamp, get by Stopwatch.GetTimestamp()</param>
    /// <param name="endTimestamp">endTimestamp, get by Stopwatch.GetTimestamp</param>
    /// <returns>elapsed time</returns>
    public static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp)
    {
#if NET
        return Stopwatch.GetElapsedTime(startTimestamp, endTimestamp);
#else
        var ticks = (long)((endTimestamp - startTimestamp) * TicksPerTimestamp);
        return new TimeSpan(ticks);
#endif
    }
}
