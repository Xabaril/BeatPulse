using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatPulse;
using BeatPulse.Core;
using BeatPulse.SqlServer;
using BeatPulse.Tracker.ApplicationInsights;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.Tracker.AppInsights
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_applicationinsights_tracker_with_instrumentationkey()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddApplicationInsightsTracker("instrumentationkey");
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.AllTrackers
                .Where(hc => hc.GetType() == typeof(ApplicationInsightsTracker))
                .Should().HaveCount(1);
        }

        [Fact]
        public void register_applicationinsights_tracker_without_instrumentationkey()
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
                .Where(hc => hc.GetType() == typeof(ApplicationInsightsTracker))
                .Should().HaveCount(1);
        }

    }
}
