﻿using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.Npgsql
{
    [Collection("execution")]
    public class npgsql_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public npgsql_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_if_npgsql_is_available()
        {
            //read appveyor services default values on
            //https://www.appveyor.com/docs/services-databases/#sql-server-2017 

            var connectionString = _fixture.IsAppVeyorExecution ?
                "Server=127.0.0.1;Port=5432;User ID=postgres;Password=Password12!;database=postgres" :
                "Server=127.0.0.1;Port=8010;User ID=postgres;Password=Password12!;database=postgres";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddNpgSql(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_if_sql_query_is_not_valid()
        {
            //read appveyor services default values on
            //https://www.appveyor.com/docs/services-databases/#sql-server-2017 

            var connectionString = _fixture.IsAppVeyorExecution ?
                "Server=127.0.0.1;Port=5432;User ID=postgres;Password=Password12!;database=postgres" :
                "Server=127.0.0.1;Port=8010;User ID=postgres;Password=Password12!;database=postgres";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddNpgSql(connectionString,"SELECT 1 FROM InvalidDb;");
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_unhealthy_if_npgsql_is_not_available()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(context =>
                   {
                       context.AddNpgSql("Server=200.0.0.1;Port=5432;User ID=postgres;Password=Password12!;database=postgres");
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
