using BeatPulse;
using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.BeatPulse.Network
{
    [Collection("execution")]
    public class dns_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public dns_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_when_configured_hosts_dns_resolution_matches()
        {
            var targetHost = "www.microsoft.com";            
            var targetHostIpAddresses = Dns.GetHostAddresses(targetHost).Select(h => h.ToString()).ToArray();

            var targetHost2 = "localhost";
            var targetHost2IpAddresses = Dns.GetHostAddresses(targetHost2).Select(h => h.ToString()).ToArray();

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddDnsResolveLiveness(options =>
                        {
                            options
                            .ResolveHost(targetHost).To(targetHostIpAddresses)
                            .ResolveHost(targetHost2).To(targetHost2IpAddresses);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_dns_resolution_does_not_match_configured_hosts()
        {           

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddDnsResolveLiveness(options =>
                        {
                            options
                            .ResolveHost("www.microsoft.com").To("8.8.8.8", "5.5.5.5");                            
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
