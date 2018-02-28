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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

            var responseStatus = response.StatusCode;

            responseStatus.Should()
                .Be(HttpStatusCode.OK);

            (await response.Content.ReadAsStringAsync()).Should()
                .Be(Enum.GetName(typeof(HttpStatusCode), responseStatus));
        }

        [Fact]
        public async Task response_http_status_ok_and_detailed_information_when_beat_pulse_service_is_healthy_and_detailed_information_is_configured()
        {
            string defaultName;
            string defaultPath;

            var healthCheck = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                httpcontext => Task.FromResult(("custom check is working", true)));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.EnableDetailedOutput())
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.Add(healthCheck);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            JsonConvert.DeserializeObject<OutputMessage>(content)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task response_http_status_serviceunavailable_when_beat_pulse_service_is_not_healthy()
        {
            string defaultName;
            string defaultPath;

            var healthCheck = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                httpcontext => Task.FromResult(("Some message when service is not available", false)));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.Add(healthCheck);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var responseStatus = response.StatusCode;

            responseStatus.Should()
                .Be(HttpStatusCode.ServiceUnavailable);

            (await response.Content.ReadAsStringAsync()).Should()
                .Be(Enum.GetName(typeof(HttpStatusCode), responseStatus));
        }

        [Fact]
        public async Task response_http_status_serviceunavailable_and_detailed_result_when_beat_pulse_service_is_not_healthy_and_detailed_is_configured()
        {
            string defaultName;
            string defaultPath;

            var healthCheck = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                httpcontext => Task.FromResult(("custom check is not working", false)));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.EnableDetailedOutput())
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
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

            var content = await response.Content.ReadAsStringAsync();

            JsonConvert.DeserializeObject<OutputMessage>(content)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task response_http_status_serviceunavailable_when_beat_pulse_service_beat_pulse_execution_is_timeout()
        {
            string defaultName;
            string defaultPath;

            var healthCheck = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                async httpcontext =>
                {
                    await Task.Delay(100);

                    return ("custom check is  working", true);
                });

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.SetTimeout(50))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.Add(healthCheck);
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var expected = HttpStatusCode.ServiceUnavailable;

            response.StatusCode
                .Should()
                .Be(expected);

            (await response.Content.ReadAsStringAsync()).Should()
                 .Be(Enum.GetName(typeof(HttpStatusCode), expected));
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
                .Configure(app =>
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
                .UseBeatPulse(o => o.SetAlternatePath("customhealthpath"))
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

        [Fact]
        public async Task response_is_not_cached_out_of_box()
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

            response.Headers.CacheControl.NoCache
                .Should().Be(true);

            response.Headers.CacheControl.MustRevalidate
                .Should().Be(true);

            response.Headers.CacheControl.NoStore
                .Should().Be(true);
        }

        [Fact]
        public async Task response_is_cached_if_enabled_on_options()
        {
            const int cacheDurationOnSeconds = 10;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.EnableOutputCache(cacheDurationOnSeconds))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.Headers.CacheControl.NoCache
                .Should().Be(false);

            response.Headers.CacheControl.Public
                .Should().Be(true);

            response.Headers.CacheControl.MaxAge
                .Should().Be(TimeSpan.FromSeconds(cacheDurationOnSeconds));

        }

        class OutputMessage
        {
            public List<dynamic> Checks { get; set; }

            public DateTime StartedAtUtc { get; set; }

            public DateTime EndAtUtc { get; set; }
        }
    }
}
