using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnitTests.Base;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BeatPulse.Core;

namespace BeatPulse
{
    public class beat_pulse_middleware_should
    { 
        [Fact]
        public async Task response_http_status_ok_when_beat_pulse_service_is_healthy()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task response_http_status_serviceunavailable_when_beat_pulse_service_is_not_healthy()
        {
            var healthCheck = new ActionHealthCheck(
                "defaultName",
                "defaultPath", 
                httpcontext => ("false", false));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context=>
                    {
                        context.Add(healthCheck);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task continue_chain_for_non_beat_pulse_requests()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                })
                .Configure(app=>
                {
                    app.Run(async ctx =>
                    {
                        await ctx.Response.WriteAsync("latest-midleware");
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync("not-beat-pulse-path");

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");
        }

        [Fact]
        public async Task continue_chain_for_not_valid_beat_pulse_http_verbs()
        {
            var webHostBuilder = new WebHostBuilder()
               .UseBeatPulse()
               .UseStartup<DefaultStartup>()
               .ConfigureServices(svc =>
               {
                   svc.AddBeatPulse();
               })
               .Configure(app =>
               {
                   app.Run(async ctx =>
                   {
                       await ctx.Response.WriteAsync("latest-midleware");
                   });
               });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .PostAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH, new StringContent(string.Empty));

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");

            response = await server.CreateClient()
                .PutAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH, new StringContent(string.Empty));

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");

            response = await server.CreateClient()
                .DeleteAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");
        }

        [Fact]
        public async Task response_http_status_ok_when_beat_pulse_service_is_healthy_on_custom_path()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(o => o.BeatPulsePath = "customhealthpath")
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync("customhealthpath");

            response.EnsureSuccessStatusCode();
        }
    }
}
