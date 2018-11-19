using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.EventStore
{
    [Collection("execution")]
    public class eventstore_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public eventstore_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task return_true_if_eventstore_is_available()
        {
            var connectionString = "tcp://localhost:1113";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddEventStore(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task return_false_if_eventstore_is_not_available()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(context =>
                   {
                       context.AddEventStore("ConnectTo=tcp://nonexistinghost:1113;HeartBeatTimeout=500;");
                   });
               });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
