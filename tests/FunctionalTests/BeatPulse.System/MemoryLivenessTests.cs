using BeatPulse;
using BeatPulse.System;
using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.BeatPulse.System
{
    [Collection("execution")]
    public class memory_liveness_should
    {
        private readonly ExecutionFixture fixture;

        public memory_liveness_should(ExecutionFixture fixture)
        {
            this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_when_private_memory_does_not_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            var maximumMemory = currentMemory + 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddPrivateMemoryLiveness(maximumMemory, Constants.PrivateMemoryLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task be_unhealthy_when_private_memory_does_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            var maximumMemory = currentMemory - 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddPrivateMemoryLiveness(maximumMemory, Constants.PrivateMemoryLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_healthy_when_workingset_does_not_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().WorkingSet64;
            var maximumMemory = currentMemory + 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddWorkingSetLiveness(maximumMemory, Constants.WorkingSetLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task be_unhealthy_when_workingset_does_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().WorkingSet64;
            var maximumMemory = currentMemory - 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddPrivateMemoryLiveness(maximumMemory, Constants.WorkingSetLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_healthy_when_virtual_memory_size_does_not_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().VirtualMemorySize64;
            var maximumMemory = currentMemory + 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddVirtualMemorySizeLiveness(maximumMemory, Constants.VirtualMemorySizeLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task be_unhealthy_when_virtual_memory_size_does_exceed_the_maximum_established()
        {
            var currentMemory = Process.GetCurrentProcess().VirtualMemorySize64;
            var maximumMemory = currentMemory - 104857600;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddVirtualMemorySizeLiveness(maximumMemory, Constants.VirtualMemorySizeLiveness);
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
