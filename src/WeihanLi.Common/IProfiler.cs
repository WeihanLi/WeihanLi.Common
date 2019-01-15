using System.Diagnostics;

namespace WeihanLi.Common
{
    public interface IProfiler
    {
        /// <summary>Starts, or resumes, measuring elapsed time for an interval.</summary>
        void Start();

        /// <summary>Stops measuring elapsed time for an interval.</summary>
        void Stop();

        /// <summary>Stops time interval measurement and resets the elapsed time to zero.</summary>
        void Reset();

        /// <summary>Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.</summary>
        void Restart();

        /// <summary>Gets the total elapsed time measured by the current instance, in milliseconds.</summary>
        long ElapsedMilliseconds { get; }
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

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}
