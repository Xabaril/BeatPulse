    using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeatPulse;
using BeatPulse.Network;
using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Xunit;

namespace FunctionalTests.BeatPulse.Network
{
    [Collection("execution")]
    public class sftp_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public sftp_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_when_connection_to_sftp_is_successful_using_password_authentication()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddSftpLiveness(options =>
                        {
                            var sftpHostConfiguration = new SftpHostConfiguration("localhost", 22);
                            sftpHostConfiguration.UsePasswordAuthentication("foo", "pass");
                            
                            options.AddHost(sftpHostConfiguration);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_unhealthy_when_connection_to_sftp_is_using_wrong_password()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddSftpLiveness(options =>
                        {
                            var sftpHostConfiguration = new SftpHostConfiguration("localhost", 22);
                            sftpHostConfiguration.UsePasswordAuthentication("foo", "wrongpass");

                            options.AddHost(sftpHostConfiguration);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task be_healthy_when_connection_to_sftp_is_successful_using_private_key()
        {
            string privateKey = File.ReadAllText("id_rsa");

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddSftpLiveness(options =>
                        {
                            var sftpHostConfiguration = new SftpHostConfiguration("localhost", 22);
                            sftpHostConfiguration.UsePrivateKeyAuthentication("foo", privateKey, "beatpulse");
                            options.AddHost(sftpHostConfiguration);
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
