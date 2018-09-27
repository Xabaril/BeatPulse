﻿using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.SqlServer
{
    [Collection("execution")]
    public class sqlserver_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public sqlserver_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_if_sqlServer_is_available()
        {
            //read appveyor services default values on
            //https://www.appveyor.com/docs/services-databases/#sql-server-2017 

            var connectionString = _fixture.IsAppVeyorExecution
                ? @"Server=(local)\SQL2016;Initial Catalog=master;User Id=sa;Password=Password12!"
                : "Server=tcp:127.0.0.1,1833;Initial Catalog=master;User Id=sa;Password=Password12!";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddSqlServer(connectionString);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_if_sqlServer_is_not_available()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(context =>
                   {
                       context.AddSqlServer("Server=tcp:200.0.0.100,1833;Initial Catalog=master;User Id=sa;Password=Password12!");
                   });
               });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_unhealthy_if_sqlquery_spec_is_not_valid()
        {
            //read appveyor services default values on
            //https://www.appveyor.com/docs/services-databases/#sql-server-2017 

            var connectionString = _fixture.IsAppVeyorExecution
                ? @"Server=(local)\SQL2016;Initial Catalog=master;User Id=sa;Password=Password12!"
                : "Server=tcp:127.0.0.1,1833;Initial Catalog=master;User Id=sa;Password=Password12!";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.AddSqlServer(connectionString, "SELECT 1 FROM [NONVALIDDB];");
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
