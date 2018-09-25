using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeatPulse;
using BeatPulse.Network;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FunctionalTests.BeatPulse.Network
{
    [Collection("execution")]
    public class ftp_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public ftp_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connection_is_successful()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddFtpLiveness(options =>
                        {
                            options.AddHost("ftp://localhost:21",
                                createFile:false,
                                credentials:new NetworkCredential("bob", "12345"));
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connection_is_successful_and_file_is_created()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddFtpLiveness(options =>
                        {
                            options.AddHost("ftp://localhost:21",
                                createFile: true,
                                credentials: new NetworkCredential("bob", "12345"));
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }
    }
}
