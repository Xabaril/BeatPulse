using BeatPulse;
using BeatPulse.Core;
using BeatPulse.StatusPageTracker;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UnitTests.Base;
using Xunit;

namespace UnitTests.BeatPulse.StatusPageTracker
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_statuspage_tracker()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddStatusPageTracker(setup => { });
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.GetAllTrackers()
                .Where(hc => hc.GetType() == typeof(StatusPageIOTracker))
                .Should().HaveCount(1);
        }
    }
}
