using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using BeatPulse;
using Microsoft.AspNetCore.TestHost;
using FluentAssertions;
using System.Net;

namespace FunctionalTests.BeatPulse.Oracle
{
    [Collection("execution")]
    public class oracle_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public oracle_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture;
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_oracle_is_available()
        {
            var connectionString = "Data Source=localhost:1521/xe;User Id=system;Password=oracle";
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddOracle(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [SkipOnAppVeyor]
        public async Task be_unhealthy_when_oracle_is_not_available()
        {
            var connectionString = "Data Source=255.255.255.255:1521/xe;User Id=system;Password=oracle";
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddOracle(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should()
                .Be(HttpStatusCode.ServiceUnavailable);
        }

        [SkipOnAppVeyor]
        public async Task be_unhealthy_when_sql_query_is_not_valid()
        {
            var connectionString = "Data Source=localhost:1521/xe;User Id=system;Password=oracle";
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddOracle(connectionString,"SELECT 1 FROM InvalidDb");
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should()
                .Be(HttpStatusCode.ServiceUnavailable);
        }

    }
}
