using System;
using System.Diagnostics;

namespace WeihanLi.Common.Helpers
{
    public sealed class ProfilerStopper : IDisposable
    {
        private readonly IProfiler _profiler;
        private readonly Action<long> _profileAction;

        public ProfilerStopper(IProfiler profiler, Action<long> profileAction)
        {
            _profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
            _profileAction = profileAction ?? throw new ArgumentNullException(nameof(profileAction));
        }

        public void Dispose()
        {
            _profiler.Stop();
            _profileAction(_profiler.ElapsedMilliseconds);
        }
    }

    public sealed class StopwatchStopper : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Action<long> _profileAction;

        public StopwatchStopper(Stopwatch stopwatch, Action<long> profileAction)
        {
            _stopwatch = stopwatch ?? throw new ArgumentNullException(nameof(stopwatch));
            _profileAction = profileAction ?? throw new ArgumentNullException(nameof(profileAction));
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _profileAction(_stopwatch.ElapsedMilliseconds);
        }
    }

    public static class ProfilerHelper
    {
        public static StopwatchStopper StartProfile(this Stopwatch watch, Action<long> profilerAction)
        {
            if (watch is null)
            {
                throw new ArgumentNullException(nameof(watch));
            }
            watch.Restart();
            return new StopwatchStopper(watch, profilerAction);
        }

        public static ProfilerStopper StartNew(this IProfiler profiler, Action<long> profilerAction)
        {
            if (profiler is null)
            {
                throw new ArgumentNullException(nameof(profiler));
            }
            profiler.Restart();
            return new ProfilerStopper(profiler, profilerAction);
        }
    }
}
