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
            var livenessOptions = new HttpLivenessOptions().WithUrl($"{HttpConfiguredTargetServer.DefaultTargetServerUrl}");
            var server = _given.AServerWithHttpLiveness(livenessOptions);

            using (var targetServer = new HttpConfiguredTargetServer(context => Task.CompletedTask))
            {
                var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

                response.EnsureSuccessStatusCode();
            };
        }

        [Fact]
        public async Task be_healthy_when_matches_configured_content_and_status_code()
        {
            var statusCode = 404;
            var content = "not found";

            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpConfiguredTargetServer.DefaultTargetServerUrl}")
                .WithContentCheck(content)
                .WithStatusCode(statusCode);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            using (var targetServer = new HttpConfiguredTargetServer(async context =>
            {
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(content);
            }))
            {
                var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();
                response.EnsureSuccessStatusCode();
            };
        }

        [Fact]
        public async Task be_unhealthy_when_configured_content_does_not_match()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpConfiguredTargetServer.DefaultTargetServerUrl}")
                .WithContentCheck("<div>Hello there!</div>");

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            using (var targetServer = new HttpConfiguredTargetServer(
                async context => await context.Response.WriteAsync("Response")))
            {
                var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

                response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Fact]
        public async Task be_unhealthy_when_configured_status_code_does_not_match()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpConfiguredTargetServer.DefaultTargetServerUrl}")
                .WithStatusCode(404);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            using(var targetServer = new HttpConfiguredTargetServer(context => Task.CompletedTask))
            {
                var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

                response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            }            
        }

        [Fact]
        public async Task be_unhealthy_when_configured_timeout_excedded()
        {
            var livenessOptions = new HttpLivenessOptions()
                .WithUrl($"{HttpConfiguredTargetServer.DefaultTargetServerUrl}")
                .WithTimeout(2);

            var server = _given.AServerWithHttpLiveness(livenessOptions);
            using(var targetServer = new HttpConfiguredTargetServer(async context => await Task.Delay(3000)))
            {
                var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH).GetAsync();

                response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            }           
        }
    }
}
