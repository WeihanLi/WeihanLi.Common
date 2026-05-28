using System.Diagnostics;

namespace WeihanLi.Common.Services;

/// <summary>
/// Measures elapsed time for an operation.
/// </summary>
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
    /// Gets the total elapsed time measured by the current instance.
    /// </summary>
    TimeSpan Elapsed { get; }
}

/// <summary>
/// <see cref="IProfiler"/> implementation backed by <see cref="Stopwatch"/>.
/// </summary>
public sealed class StopwatchProfiler : IProfiler
{
    private readonly Stopwatch _stopwatch = new();

    /// <summary>
    /// Starts or resumes measuring elapsed time.
    /// </summary>
    public void Start()
    {
        _stopwatch.Start();
    }

    /// <summary>
    /// Stops measuring elapsed time.
    /// </summary>
    public void Stop()
    {
        _stopwatch.Stop();
    }

    /// <summary>
    /// Resets elapsed time to zero and starts measuring elapsed time.
    /// </summary>
    public void Restart()
    {
        _stopwatch.Restart();
    }

    /// <summary>
    /// Gets the total elapsed time measured by the profiler.
    /// </summary>
    public TimeSpan Elapsed => _stopwatch.Elapsed;
}
