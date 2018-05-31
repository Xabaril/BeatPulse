using BeatPulse;
using BeatPulse.Core;
using BeatPulse.PrometheusTracker;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Base;
using Xunit;

namespace UnitTests.BeatPulse.PrometheusTracker
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_prometheus_tracker()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddPrometheusTracker(new Uri("http://prometheysgatwayuri"));
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.AllTrackers
                .Where(hc => hc.GetType() == typeof(PrometheusGatewayTracker))
                .Should().HaveCount(1);
        }

        [Fact]
        public void register_prometheus_tracker_with_custom_labels()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddPrometheusTracker(new Uri("http://prometheysgatwayuri"), new Dictionary<string,string>());
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.AllTrackers
                .Where(hc => hc.GetType() == typeof(PrometheusGatewayTracker))
                .Should().HaveCount(1);
        }
    }
}
