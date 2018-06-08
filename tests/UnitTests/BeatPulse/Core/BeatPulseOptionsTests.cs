using BeatPulse;
using FluentAssertions;
using System.Diagnostics;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class beat_pulse_options_should
    {
        [Fact]
        public void set_default_parameters_when_is_created()
        {
            var watch = new Stopwatch();
            watch.Start();
            var highResolution = Stopwatch.IsHighResolution;

            watch.Stop();

            var i = watch.ElapsedMilliseconds;

            var options = new BeatPulseOptions();

            options.BeatPulsePath
                .Should().Be(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            options.Timeout
                .Should().Be(-1);

            options.DetailedOutput
                .Should().Be(false);

            options.CacheOutput
                .Should().Be(false);

            options.CacheDuration
                .Should().Be(0);

            options.CacheMode
                .Should().Be(CacheMode.Header);
        }

        [Fact]
        public void change_default_beatpulse_path()
        {
            var options = new BeatPulseOptions();

            options.BeatPulsePath
                .Should().Be(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            options.ConfigurePath("health");

            options.BeatPulsePath
               .Should().Be("health");
        }

        [Fact]
        public void change_default_timeout()
        {
            var options = new BeatPulseOptions();

            options.Timeout
                .Should().Be(-1);

            options.ConfigureTimeout(1000);

            options.Timeout
               .Should().Be(1000);
        }

        [Fact]
        public void change_default_cachemode()
        {
            var options = new BeatPulseOptions();

            options.CacheOutput
                .Should().Be(false);

            options.ConfigureOutputCache(10, CacheMode.HeaderAndServerMemory);

            options.CacheOutput
               .Should().Be(true);

            options.CacheMode
               .Should().Be(CacheMode.HeaderAndServerMemory);

            options.CacheDuration
                .Should().Be(10);
        }
    }
}
