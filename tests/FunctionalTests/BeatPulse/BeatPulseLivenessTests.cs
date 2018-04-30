using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BeatPulse.Core;
using Xunit;

namespace BeatPulse
{
    [Collection("execution")]
    public class liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task return_all_checks_results_when_EnableDetailedOutput_is_true()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse(options => options.EnableDetailedOutput())
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        var httpClient = new HttpClient();

                        context.Add(new ActionLiveness("nok", "nok", async (httpContext, token) =>
                        {
                            var responseMessage = await httpClient.GetAsync("http://notexist/api", token);

                            return responseMessage.IsSuccessStatusCode 
                                ? ("OK", true) 
                                : ("the not exist api is broken!", false);
                        }));

                        context.Add(new ActionLiveness("ok", "ok", async (httpContext, token) =>
                        {
                            var responseMessage = await httpClient.GetAsync("http://www.google.es", token);

                            return responseMessage.IsSuccessStatusCode
                                ? ("OK", true)
                                : ("the not exist api is broken!", false);
                        }));
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("\"Name\":\"self\"");
            body.Should().Contain("\"Name\":\"nok\"");
            body.Should().Contain("\"Name\":\"ok\"");
        }

        [Fact]
        public async Task return_plain_text_result_when_EnableDetailedOutput_is_false()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        var httpClient = new HttpClient();

                        context.Add(new ActionLiveness("nok", "nok", async (httpContext, token) =>
                        {
                            var responseMessage = await httpClient.GetAsync("http://notexist/api", token);

                            return responseMessage.IsSuccessStatusCode
                                ? ("OK", true)
                                : ("the not exist api is broken!", false);
                        }));

                        context.Add(new ActionLiveness("ok", "ok", async (httpContext, token) =>
                        {
                            var responseMessage = await httpClient.GetAsync("http://www.google.es", token);

                            return responseMessage.IsSuccessStatusCode
                                ? ("OK", true)
                                : ("the not exist api is broken!", false);
                        }));
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
            response.Content.Headers.ContentType.MediaType.Should().Be("text/plain");

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Be("ServiceUnavailable");
        }
    }
}
