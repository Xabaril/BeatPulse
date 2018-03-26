using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using BeatPulse;
using Microsoft.AspNetCore.TestHost;
using FluentAssertions;
using System.Net;

namespace FunctionalTests.BeatPulse.MySql
{
    [Collection("execution")]
    public class mysql_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public mysql_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_when_mysql_server_is_available()
        {
            var connectionString = "server=localhost;port=3306;database=information_schema;uid=root;password=Password12!";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddMySql(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_mysql_server_is_unavailable()
        {
            var connectionString = "server=255.255.255.255;port=3306;database=information_schema;uid=root;password=Password12!";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddMySql(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
