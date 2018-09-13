using BeatPulse.Core;
using FluentAssertions;
using System;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class clock_should
    {
        [Fact]
        public void get_elapsed_time_with_precission()
        {
            var clock = Clock.StartNew();
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow < startTime.AddSeconds(1)) { }

            var elapsed = clock.Elapsed();

            elapsed.Should()
                .BeCloseTo(TimeSpan.FromSeconds(1));            
        }
    }
}
