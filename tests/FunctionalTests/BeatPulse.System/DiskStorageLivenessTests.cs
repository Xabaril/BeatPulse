using FunctionalTests.Base;
using System.Threading.Tasks;
using Xunit;
using BeatPulse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using FluentAssertions;

namespace FunctionalTests.BeatPulse.System
{
    [Collection("execution")]
    public class disk_storage_liveness_should
    {
        private readonly ExecutionFixture _fixture;
        private DriveInfo[] _drives;
        public disk_storage_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture;
            _drives = DriveInfo.GetDrives();
        }

        [Fact]
        public async Task be_healthy_when_disks_have_more_free_space_than_configured()
        {
            var testDrive = _drives.FirstOrDefault(d => d.DriveType == DriveType.Fixed);

            var testDriveActualFreeMegabytes = testDrive.AvailableFreeSpace / 1024 / 1024;
            var targetFreeSpace = testDriveActualFreeMegabytes - 50;

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(options =>
                    {
                        options.AddDiskStorageLiveness(setup =>
                        {
                            setup.AddDrive(testDrive.Name, targetFreeSpace);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
              .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_a_disk_has_less_free_space_than_configured()
        {
            var testDrive = _drives.FirstOrDefault(d => d.DriveType == DriveType.Fixed);

            var testDriveActualFreeMegabytes = testDrive.AvailableFreeSpace / 1024 / 1024;
            var targetFreeSpace = testDriveActualFreeMegabytes + 50;

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(options =>
                    {
                        options.AddDiskStorageLiveness(setup =>
                        {
                            setup.AddDrive(testDrive.Name, targetFreeSpace);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
              .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_unhealthy_when_a_configured_disk_does_not_exist()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(options =>
                    {
                        options.AddDiskStorageLiveness(setup =>
                        {
                            setup.AddDrive("nonexistingdisk", 104857600);
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