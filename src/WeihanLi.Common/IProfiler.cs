using System.Diagnostics;

namespace WeihanLi.Common;

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

public class StopwatchProfiler : IProfiler
{
    private readonly Stopwatch _stopwatch;

    public StopwatchProfiler()
    {
        _stopwatch = new Stopwatch();
    }

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
