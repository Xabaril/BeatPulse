using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Network;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace UnitTests.BeatPulse.Network
{
    public class beatpulse_context_should
    {
        [Fact]
        public void register_ping_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddPingLiveness(options =>
                        {
                            options.AddHost("fakehost", 5000);
                        });
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should().ContainsLiveness(nameof(PingLiveness));

        }
    }
}
