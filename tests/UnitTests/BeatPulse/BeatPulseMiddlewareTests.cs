using BeatPulse.Core;
using BeatPulse.Core.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnitTests.Base;
using Xunit;

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

            var healthCheck = new ActionLiveness((cancellationToken) => Task.FromResult(LivenessResult.Healthy()));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput())
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddLiveness(nameof(defaultName), opt =>
                        {
                            opt.UsePath(nameof(defaultPath));
                            opt.UseLiveness(healthCheck);
                        });
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

            var healthCheck = new ActionLiveness((cancellationToken) => Task.FromResult(LivenessResult.UnHealthy("Some message when service is not available")));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddLiveness(nameof(defaultName), opt =>
                        {
                            opt.UsePath(nameof(defaultPath));
                            opt.UseLiveness(healthCheck);
                        });
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

            var healthCheck = new ActionLiveness((cancellationToken) => Task.FromResult(LivenessResult.UnHealthy("custom check is not working")));

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput())
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddLiveness(nameof(defaultName), opt =>
                        {
                            opt.UsePath(nameof(defaultPath));
                            opt.UseLiveness(healthCheck);
                        });
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
                async (cancellationToken) =>
                {
                    await Task.Delay(100);

                    return LivenessResult.Healthy();
                });

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureTimeout(50))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddLiveness(nameof(defaultName), opt =>
                        {
                            opt.UsePath(nameof(defaultPath));
                            opt.UseLiveness(healthCheck);
                        });
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
                .UseBeatPulse(o => o.ConfigurePath("customhealthpath"))
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
        public async Task response_content_type_is_text_plain_if_detailed_output_is_disabled()
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

            response.Content.Headers.ContentType.MediaType.Should().Be("text/plain");
        }

        [Fact]
        public async Task break_the_liveness_execution_if_detailed_output_is_disabled()
        {
            var check1IsExecuted = false;
            var check2IsExecuted = false;

            var healthCheck1 = new ActionLiveness(
               (cancellationToken) =>
               {
                   check1IsExecuted = true;
                   return Task.FromResult(LivenessResult.UnHealthy("custom check1 is not working"));
               });

            var healthCheck2 = new ActionLiveness(
              (cancellationToken) =>
              {
                  check2IsExecuted = false;
                  return Task.FromResult(LivenessResult.Healthy());
              });

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddLiveness("check1", opt =>
                        {
                            opt.UsePath("check1");
                            opt.UseLiveness(healthCheck1);
                        });
                        context.AddLiveness("check2", opt =>
                        {
                            opt.UsePath("check2");
                            opt.UseLiveness(healthCheck2);
                        });
                    });

                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.StatusCode
                .Should().Be(StatusCodes.Status503ServiceUnavailable);

            check1IsExecuted.Should().BeTrue();
            check2IsExecuted.Should().BeFalse();
        }

        [Fact]
        public async Task response_content_type_is_application_json_if_detailed_output_is_enabled()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput())
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
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
        public async Task response_is_cached_if_enabled_on_options_using_headers_by_default()
        {
            const int cacheDurationOnSeconds = 10;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureOutputCache(cacheDurationOnSeconds))
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

        [Fact]
        public async Task execute_tests_if_requested_by_user_agent_even_though_response_is_cached_by_headers()
        {
            const int cacheDurationOnSeconds = 10;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options =>
                {
                    options.ConfigureOutputCache(cacheDurationOnSeconds);
                    options.ConfigureDetailedOutput();
                })
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var firstResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var secondResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var firstJson = await firstResponse.Content.ReadAsStringAsync();
            var secondJson = await secondResponse.Content.ReadAsStringAsync();

            firstJson.Equals(secondJson).Should().Be(false);

        }

        [Fact]
        public async Task use_in_memory_cache_if_specified_in_options()
        {
            const int cacheDurationOnSeconds = 10;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options =>
                {
                    options.ConfigureOutputCache(cacheDurationOnSeconds, CacheMode.ServerMemory);
                    options.ConfigureDetailedOutput();
                })
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var firstResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var secondResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var firstJson = await firstResponse.Content.ReadAsStringAsync();
            var secondJson = await secondResponse.Content.ReadAsStringAsync();

            firstJson.Equals(secondJson).Should().Be(true);
        }

        [Fact]
        public async Task do_not_return_in_memory_cached_response_if_time_has_passed()
        {
            const int cacheDurationOnSeconds = 1;

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options =>
                {
                    options.ConfigureOutputCache(cacheDurationOnSeconds, CacheMode.ServerMemory);
                    options.ConfigureDetailedOutput();
                })
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var firstResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            await Task.Delay(1500);

            var secondResponse = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            var firstJson = await firstResponse.Content.ReadAsStringAsync();
            var secondJson = await secondResponse.Content.ReadAsStringAsync();

            firstJson.Equals(secondJson).Should().BeFalse();
        }

        [Fact]
        public async Task response_http_status_not_found_if_beatpulse_path_is_not_registered()
        {

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH + "/not-registered-path");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task response_should_be_unauthorized_when_authorization_filter_configured_and_apikey_does_not_match()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddSingleton<IBeatPulseAuthenticationFilter>(new ApiKeyAuthenticationFilter("the-api-key"));
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?api-key=test");

            response.StatusCode.Should()
                .Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task response_should_be_ok_when_authorization_filter_validates_request_api_key()
        {
            const string testApiKey = "1234";

            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddSingleton<IBeatPulseAuthenticationFilter>(new ApiKeyAuthenticationFilter($"{testApiKey}"));
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?api-key={testApiKey}");

            response.StatusCode.Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task response_should_add_access_control_headers_when_cors_is_configured_in_pulse_pipeline()
        {
            const string origin = "test-api.com";

            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .ConfigureServices(svc =>
               {
                   svc.AddCors();
                   svc.AddBeatPulse();
                   svc.AddMvc();
               }).Configure(app =>
               {
                   app.UseBeatPulse(setup => { }, builder =>
                    {
                        builder.UseCors(setup =>
                        {
                            setup.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials();
                        });
                    });
                   app.UseMvc();
               });

            var server = new TestServer(webHostBuilder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Origin", origin);
            var response = await client.GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}");

            response.Headers.FirstOrDefault(s => s.Key == "Access-Control-Allow-Origin")
                .Value.Should().BeEquivalentTo(origin);
        }

        [Fact]
        public async Task response_should_add_access_control_headers_when_cors_is_configured_in_global_pipeline()
        {
            const string origin = "test-api.com";

            var webHostBuilder = new WebHostBuilder()
               .UseStartup<DefaultStartup>()
               .ConfigureServices(svc =>
               {
                   svc.AddCors();
                   svc.AddBeatPulse();
                   svc.AddMvc();
               }).Configure(app =>
               {
                   app.UseCors(setup =>
                   {
                       setup.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                   });
                   app.UseBeatPulse(setup => { });
                   app.UseMvc();
               });

            var server = new TestServer(webHostBuilder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Origin", origin);
            var response = await client.GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}");

            response.Headers.FirstOrDefault(s => s.Key == "Access-Control-Allow-Origin")
                .Value.Should().BeEquivalentTo(origin);
        }

        [Fact]
        public async Task response_should_not_add_access_control_headers_for_a_different_origin_if_cors_not_configured()
        {
            const string origin = "test-api.com";

            var webHostBuilder = new WebHostBuilder()
               .UseBeatPulse()
               .UseStartup<DefaultStartup>()
               .ConfigureServices(svc =>
               {
                   svc.AddCors();
                   svc.AddBeatPulse();
                   svc.AddMvc();
               })
               .Configure(app =>
               {
                   app.UseCors(setup =>
                   {
                       setup.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                   });
                   app.UseMvc();
               });

            var server = new TestServer(webHostBuilder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Origin", origin);
            var response = await client.GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}");

            response.Headers.FirstOrDefault(s => s.Key == "Access-Control-Allow-Origin")
                .Value.Should().BeEquivalentTo(null);
        }

        [Fact]
        public async Task use_detailed_errors_configuration_property_on_liveness_exection_context()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options =>
                {
                    options.ConfigureDetailedOutput(detailedOutput: true, includeExceptionMessages: true);
                })
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(ctx =>
                    {
                        ctx.AddLiveness("test", setup =>
                         {
                             setup.UsePath("test");
                             setup.UseLiveness(new LivenessThatThrow(() => new ArgumentException("detail")));
                         });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync(BeatPulseKeys.BEATPULSE_DEFAULT_PATH);

            response.Content.Headers
                .ContentType
                .MediaType
                .Should()
                .Be("application/json");

            var responseContent = await response.Content
                .ReadAsStringAsync();

            var exception = (string)JsonConvert.DeserializeObject<dynamic>(responseContent)
                .checks[1].exception;

            var message = (string)JsonConvert.DeserializeObject<dynamic>(responseContent)
                .checks[1].message;

            exception.Should()
                .BeEquivalentTo(new ArgumentException("detail").Message);

            message.Should()
                .BeEquivalentTo(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE);
        }

        [Fact]
        public async Task default_detailedOutput_option_can_be_override_with_querystring_parameter()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput(detailedOutput: false))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?DetailedOutput=true");

            response.Content.Headers
                .ContentType
                .MediaType
                .Should()
                .Be("application/json");
        }

        [Fact]
        public async Task default_timeout_option_can_be_override_with_querystring_parameter()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureTimeout(milliseconds: 100))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(setup =>
                    {
                        setup.AddLiveness("test", opt =>
                         {
                             opt.UsePath("test");
                             opt.UseLiveness(new ActionLiveness(async ct =>
                             {
                                 await Task.Delay(200);

                                 return LivenessResult.Healthy();
                             }));
                         });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?DetailedOutput=true");

            response.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);

            response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?DetailedOutput=true&Timeout=300");

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task default_detailedOutput_option_can_be_override_with_querystring_parameters_without_same_case()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput(detailedOutput: false))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?DeTaIlEdOuTpUt=true");

            response.Content.Headers
                .ContentType
                .MediaType
                .Should()
                .Be("application/json");
        }

        [Fact]
        public async Task default_detailedOutput_option_can_be_override_with_latest_querystring_parameter_entry_if_multiple_entries_are_sent()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse(options => options.ConfigureDetailedOutput(detailedOutput: false))
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse();
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateClient()
                .GetAsync($"{BeatPulseKeys.BEATPULSE_DEFAULT_PATH}?DetailedOutput=false&DetailedOutput=false&DetailedOutput=true");

            response.Content.Headers
                .ContentType
                .MediaType
                .Should()
                .Be("application/json");
        }

        class OutputMessage
        {
            public List<dynamic> Checks { get; set; }

            public DateTime StartedAtUtc { get; set; }

            public DateTime EndAtUtc { get; set; }
        }

        class LivenessThatThrow
            : IBeatPulseLiveness
        {
            Func<Exception> _throwException;

            public LivenessThatThrow(Func<Exception> thowException)
            {
                _throwException = thowException;
            }

            public Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    throw _throwException();
                }
                catch (Exception ex)
                {
                    return Task.FromResult(
                        LivenessResult.UnHealthy(ex));
                }
            }
        }
    }
}
