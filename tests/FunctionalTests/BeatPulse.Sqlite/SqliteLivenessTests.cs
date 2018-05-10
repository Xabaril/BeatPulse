using BeatPulse;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.IO;
using Xunit;
using BeatPulse.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;

namespace FunctionalTests.BeatPulse.Sqlite
{
    [Collection("execution")]
    public class sqllite_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public sqllite_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void be_healthy_when_sqlite_is_available()
        {

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddSqlite($"Data Source=sqlite.db", healthQuery: "select name from sqlite_master where type='table'");
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_sqlite_is_unavailable()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddSqlite("Data Source=fake.db", healthQuery: "Select * from Users");
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
