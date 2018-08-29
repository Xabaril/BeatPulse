using BeatPulse;
using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.BeatPulse.Network
{
    [Collection("execution")]
    public class imap_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public imap_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connecting_to_imap_ssl_port_without_login()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(setup =>
                   {
                       setup.AddImapLiveness(options =>
                       {
                           options.Host = "localhost";
                           options.Port = 993;
                           options.UseSsl = true;
                           options.AllowInvalidRemoteCertificates = true;
                       });
                   });
               });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connecting_to_imap_ssl_and_login_with_correct_account()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(setup =>
                   {
                       setup.AddImapLiveness(options =>
                       {
                           options.Host = "localhost";
                           options.Port = 993;
                           options.UseSsl = true;
                           options.AllowInvalidRemoteCertificates = true;
                           options.LoginWith("admin@beatpulse.com", "beatpulse");
                       });
                   });
               });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [SkipOnAppVeyor]
        public async Task be_unhealthy_when_connecting_to_imap_ssl_and_login_with_an_incorrect_account()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(setup =>
                   {
                       setup.AddImapLiveness(options =>
                       {
                           options.Host = "localhost";
                           options.Port = 993;
                           options.UseSsl = true;
                           options.AllowInvalidRemoteCertificates = true;
                           options.LoginWith("admin@beatpulse.com", "invalidpassword");
                       });
                   });
               });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connecting_to_imap_ssl_with_a_correct_account_checking_an_existing_folder()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(setup =>
                   {
                       setup.AddImapLiveness(options =>
                       {
                           options.Host = "localhost";
                           options.Port = 993;
                           options.UseSsl = true;
                           options.AllowInvalidRemoteCertificates = true;
                           options.LoginWith("admin@beatpulse.com", "beatpulse");
                           options.CheckFolderExists("INBOX");
                       });
                   });
               });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }


        [SkipOnAppVeyor]
        public async Task be_unhealthy_when_connecting_to_imap_ssl_with_a_correct_account_checking_an_non_existing_folder()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .UseBeatPulse()
               .ConfigureServices(services =>
               {
                   services.AddBeatPulse(setup =>
                   {
                       setup.AddImapLiveness(options =>
                       {
                           options.Host = "localhost";
                           options.Port = 993;
                           options.UseSsl = true;
                           options.AllowInvalidRemoteCertificates = true;
                           options.LoginWith("admin@beatpulse.com", "beatpulse");
                           options.CheckFolderExists("INVALIDFOLDER");
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
