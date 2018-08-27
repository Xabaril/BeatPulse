using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Network;
using BeatPulse.System;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace UnitTests.BeatPulse.System
{
    public class beat_pulse_context_should
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

        [Fact]
        public void register_disk_storage_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddDiskStorageLiveness(options =>
                        {
                            options.AddDrive("C:\\", 5000);
                        });
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should().ContainsLiveness(nameof(DiskStorageLiveness));
        }

        [Fact]
        public void register_memory_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context
                            .AddPrivateMemoryLiveness(104857600, Constants.PrivateMemoryLiveness)
                            .AddWorkingSetLiveness(104857600, Constants.WorkingSetLiveness)
                            .AddVirtualMemorySizeLiveness(104857600, Constants.VirtualMemorySizeLiveness); 
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should().ContainsLiveness(Constants.PrivateMemoryLiveness);
            beatPulseContext.Should().ContainsLiveness(Constants.WorkingSetLiveness);
            beatPulseContext.Should().ContainsLiveness(Constants.VirtualMemorySizeLiveness);
        }
    }
}
