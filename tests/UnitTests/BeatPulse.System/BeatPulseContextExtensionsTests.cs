using Microsoft.AspNetCore.Hosting;
using BeatPulse;
using UnitTests.Base;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using BeatPulse.System;
using Microsoft.AspNetCore.TestHost;
using BeatPulse.Core;
using System.Linq;
using FluentAssertions;

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

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.GetAllLiveness("ping")
                .Where(hc => hc.Name == nameof(PingLiveness))
                .Should().HaveCount(1);

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

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.GetAllLiveness("diskstorage")
                .Where(hc => hc.Name == nameof(DiskStorageLiveness))
                .Should().HaveCount(1);

        }
    }
}
