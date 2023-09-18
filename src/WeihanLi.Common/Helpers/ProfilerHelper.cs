using System.Diagnostics;
using WeihanLi.Common.Services;

namespace WeihanLi.Common.Helpers;

public sealed class ProfilerStopper : IDisposable
{
    private readonly IProfiler _profiler;
    private readonly Action<TimeSpan> _profileAction;

    public ProfilerStopper(IProfiler profiler, Action<TimeSpan> profileAction)
    {
        _profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
        _profileAction = profileAction ?? throw new ArgumentNullException(nameof(profileAction));
    }

    public void Dispose()
    {
        _profiler.Stop();
        _profileAction(_profiler.Elapsed);
    }
}

public sealed class StopwatchStopper : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly Action<TimeSpan> _profileAction;

    public StopwatchStopper(Stopwatch stopwatch, Action<TimeSpan> profileAction)
    {
        _stopwatch = stopwatch ?? throw new ArgumentNullException(nameof(stopwatch));
        _profileAction = profileAction ?? throw new ArgumentNullException(nameof(profileAction));
    }

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
        Guard.NotNull(watch, nameof(watch)).Restart();
        return new StopwatchStopper(watch, profilerAction);
    }

    public static ProfilerStopper StartNew(this IProfiler profiler, Action<TimeSpan> profilerAction)
    {
        Guard.NotNull(profiler, nameof(profiler)).Restart();
        return new ProfilerStopper(profiler, profilerAction);
    }

    public static readonly double TicksPerTimestamp = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    /// <summary>
    /// GetElapsedTime
    /// </summary>
    /// <param name="startTimestamp">startTimestamp, get by Stopwatch.GetTimestamp()</param>
    /// <returns>elapsed timespan</returns>
    public static TimeSpan GetElapsedTime(long startTimestamp) =>
#if NET7_0_OR_GREATER
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
    /// <returns>elapsed timespan</returns>
    public static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp)
    {
#if NET7_0_OR_GREATER
        return Stopwatch.GetElapsedTime(startTimestamp, endTimestamp);
#else
        var ticks = (long)((endTimestamp - startTimestamp) * TicksPerTimestamp);
        return new TimeSpan(ticks);
#endif
    }
}
