using BeatPulse.Core;
using BeatPulse.Uris;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UnitTests.Base;
using Xunit;
using BeatPulse;
using System;

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

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.GetAllLiveness("uri-group")
                .Where(hc => hc.Name == nameof(UriLiveness))
                .Should().HaveCount(1);

        }
    }
}
