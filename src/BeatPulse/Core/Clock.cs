using System;
using System.Diagnostics;

namespace BeatPulse.Core
{
    class Clock
    {
        private const long TicksMultiplier = 1000 * TimeSpan.TicksPerMillisecond;

        private readonly DateTime _startTime;
        private readonly double _startTimestamp;

        private Clock()
        {
            _startTime = DateTime.UtcNow;
            _startTimestamp = Stopwatch.GetTimestamp();
        }

        public static Clock StartNew()
        {
            return new Clock();
        }

        public TimeSpan Elapsed()
        {
            var endTimestamp = Stopwatch.GetTimestamp();
            var durationInTicks = ((endTimestamp - _startTimestamp) / Stopwatch.Frequency) * TicksMultiplier;

            return TimeSpan.FromTicks((long)durationInTicks);
        }
    }
}
