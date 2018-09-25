using BeatPulse;
using BeatPulse.Network.Core;
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
    public class smtp_liveness_should
    {
        private readonly ExecutionFixture _fixture;
       
        //Host and login account to fast switch tests against different server
        private string _host = "localhost";
        private string _validAccount = "admin@beatpulse.com";
        private string _validPassword = "beatpulse";

        public smtp_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connecting_using_ssl()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<DefaultStartup>()
             .UseBeatPulse()
             .ConfigureServices(services =>
             {
                 services.AddBeatPulse(setup =>
                 {
                     setup.AddSmtpLiveness(options =>
                     {
                         //SSL on by default
                         options.Host = _host;
                         options.Port = 465;
                         options.ConnectionType = SmtpConnectionType.SSL;
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
        public async Task be_healthy_when_connecting_using_tls()
        {
            var webHostBuilder = new WebHostBuilder()
             .UseStartup<DefaultStartup>()
             .UseBeatPulse()
             .ConfigureServices(services =>
             {
                 services.AddBeatPulse(setup =>
                 {
                     setup.AddSmtpLiveness(options =>
                     {
                         options.Host = _host;
                         options.Port = 587;
                         options.ConnectionType = SmtpConnectionType.TLS;
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
        public async Task be_healthy_when_connecting_using_connection_type_auto()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(setup =>
               {
                   setup.AddSmtpLiveness(options =>
                   {
                       options.Host = _host;
                       options.Port = 587;
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
        public async Task be_unhealthy_when_connecting_to_an_invalid_smtp_port_with_mode_auto()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(setup =>
               {
                   setup.AddSmtpLiveness(options =>
                   {
                       options.Host = _host;
                       options.Port = 45;
                       options.AllowInvalidRemoteCertificates = true;
                   });
               });
           });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_when_connection_and_login_with_valid_account_using_ssl_port_and_mode_auto()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(setup =>
               {
                   setup.AddSmtpLiveness(options =>
                   {
                       options.Host = _host;
                       options.Port = 465;
                       options.AllowInvalidRemoteCertificates = true;
                       options.LoginWith(_validAccount, _validPassword);
                   });
               });
           });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }


        [SkipOnAppVeyor]
        public async Task be_healthy_when_connection_and_login_with_valid_account_using_tls_port_and_mode_auto()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(setup =>
               {
                   setup.AddSmtpLiveness(options =>
                   {
                       options.Host = _host;
                       options.Port = 587;
                       options.AllowInvalidRemoteCertificates = true;
                       options.LoginWith(_validAccount, _validPassword);
                   });
               });
           });

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();

        }


        [SkipOnAppVeyor]
        public async Task be_unhealthy_when_connection_and_login_with_an_invalid_account()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(setup =>
               {
                   setup.AddSmtpLiveness(options =>
                   {
                       options.Host = _host;
                       options.Port = 587;
                       options.AllowInvalidRemoteCertificates = true;
                       options.LoginWith(_validAccount, "wrongpass");
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
