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

namespace FunctionalTests.BeatPulse.IdSvr
{
    [Collection("execution")]
    public class idsvr_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public idsvr_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task return_false_if_idsvr_is_unavailable()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<DefaultStartup>()
              .UseBeatPulse()
              .ConfigureServices(services =>
              {
                  services.AddBeatPulse(context =>
                  {
                      context.AddIdentityServer(new Uri("http://localhost:7777"));
                  });
              });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);

        }
        [SkipOnAppVeyor]
        public async Task return_true_if_idsvr_is_available()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .UseBeatPulse()
           .ConfigureServices(services =>
           {
               services.AddBeatPulse(context =>
               {
                   context.AddIdentityServer(new Uri("http://localhost:8888"));
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
