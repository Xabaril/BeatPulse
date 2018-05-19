using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.ApplicationInsightsTracker
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_applicationinsights_tracker()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddApplicationInsightsTracker();
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.AllTrackers
                .Where(hc => hc.GetType() == typeof(AITracker))
                .Should().HaveCount(1);
        }
    }
}
