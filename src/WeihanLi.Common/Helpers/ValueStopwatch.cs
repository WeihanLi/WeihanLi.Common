using System;
using System.Diagnostics;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// Value-type replacement for <see cref="Stopwatch"/> which avoids allocations.
    /// </summary>
    /// <remarks>
    /// Inspired on <seealso href="https://github.com/dotnet/extensions/blob/master/src/Shared/src/ValueStopwatch/ValueStopwatch.cs"/>.
    /// </remarks>
    public struct ValueStopwatch
    {
        private static readonly double _timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private long _startTimestamp, _stopTimestamp;

        private ValueStopwatch(long startTimestamp)
        {
            _startTimestamp = startTimestamp;
            _stopTimestamp = 0;
        }

        /// <summary>
        /// Gets the time elapsed since the stopwatch was created with <see cref="StartNew"/>.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                var end = _stopTimestamp > 0 ? _stopTimestamp : Stopwatch.GetTimestamp();
                var timestampDelta = end - _startTimestamp;
                var ticks = (long)(_timestampToTicks * timestampDelta);
                return new TimeSpan(ticks);
            }
        }

        public void Restart()
        {
            _startTimestamp = Stopwatch.GetTimestamp();
            _stopTimestamp = 0;
        }

        public void Stop() => _stopTimestamp = Stopwatch.GetTimestamp();

        /// <summary>
        /// Creates a new <see cref="ValueStopwatch"/> that is ready to be used.
        /// </summary>
        public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());
    }
}
