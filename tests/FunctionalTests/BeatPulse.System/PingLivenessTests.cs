using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using BeatPulse.System;
using Microsoft.AspNetCore.TestHost;
using BeatPulse;
using FluentAssertions;
using System.Net;

namespace FunctionalTests.BeatPulse.System
{
    [Collection("execution")]
    public class ping_liveness_should
    {
        private readonly ExecutionFixture _fixture;
        public ping_liveness_should(ExecutionFixture fixture) => _fixture = fixture;
       
        [Fact]
        public async Task be_healthy_when_all_hosts_replies_to_ping()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
               {
                   services.AddBeatPulse(options =>
                   {
                       options.AddPingLiveness(setup =>
                       {
                           setup.AddHost("127.0.0.1", 5000);
                       });
                   });
               });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
              .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_a_host_ping_is_not_successful()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(options =>
                    {
                        options.AddPingLiveness(setup =>
                        {
                            setup.AddHost("127.0.0.1", 3000);
                            setup.AddHost("nonexistinghost", 3000);
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
