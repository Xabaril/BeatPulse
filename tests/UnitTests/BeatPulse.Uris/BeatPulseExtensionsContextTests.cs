using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Uris;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using UnitTests.Base;
using Xunit;

namespace UnitTests.BeatPulse.Uris
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_uri_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddUrlGroup(new Uri("https://www.your-uri-here.com"));
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should()
                .ContainsLiveness(nameof(UriLiveness));
        }
    }
}
