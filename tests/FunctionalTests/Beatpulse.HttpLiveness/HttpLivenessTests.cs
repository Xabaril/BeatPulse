using BeatPulse.Core.Http;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using BeatPulse;
using FunctionalTests.Beatpulse.Http.Fixtures;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System.Net;

namespace FunctionalTests.Beatpulse.Http
{
    [Collection("execution")]
    public class http_liveness_should
    {
        private readonly ExecutionFixture _fixture;
        private readonly HttpLivenessGivenFixture _given = new HttpLivenessGivenFixture();
        

        public http_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task be_healthy_when_success_response_from_endpoint_with_default_options()
        {
            var livenessOptions = new HttpLivenessOptions().WithUrl($"{HttpLivenessGivenFixture.DefaultTargetServerUrl}");

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            var targetServer = _given.ATargetServerWithConfiguredResponse(context => Task.CompletedTask);
            await targetServer.StartAsync();
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

            response.EnsureSuccessStatusCode();

            await targetServer.StopAsync();
        }

        [Fact]
        public async Task be_healthy_when_matches_configured_content_and_status_code()
        {
            var statusCode = 301;
            var content = "redirect";

            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpLivenessGivenFixture.DefaultTargetServerUrl}")
                .WithContentCheck(content)
                .WithStatusCode(statusCode);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            var targetServer = _given.ATargetServerWithConfiguredResponse
                (async context => {
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(content);
                });

            await targetServer.StartAsync();
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

            await targetServer.StopAsync();
        }



        [Fact]
        public async Task be_unhealthy_when_configured_content_does_not_match()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpLivenessGivenFixture.DefaultTargetServerUrl}")
                .WithContentCheck("<div>Hello there!</div>");

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            var targetServer = _given.ATargetServerWithConfiguredResponse
                (async context => await context.Response.WriteAsync("Response"));

            await targetServer.StartAsync();
            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
      
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

            await targetServer.StopAsync();
        }      

        [Fact]
        public async Task be_unhealthy_when_configured_status_code_does_not_match()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpLivenessGivenFixture.DefaultTargetServerUrl}")
                .WithStatusCode(404);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            var targetServer = _given.ATargetServerWithConfiguredResponse( context => Task.CompletedTask);
            await targetServer.StartAsync();

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
            
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

            await targetServer.StopAsync();
        }

        [Fact]
        public async Task be_unhealthy_when_configured_timeout_excedded()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpLivenessGivenFixture.DefaultTargetServerUrl}")
                .WithTimeout(2);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            var targetServer = _given.ATargetServerWithConfiguredResponse
                (async context => await Task.Delay(3000));
            await targetServer.StartAsync();

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            await targetServer.StopAsync();
        }
    }
}
