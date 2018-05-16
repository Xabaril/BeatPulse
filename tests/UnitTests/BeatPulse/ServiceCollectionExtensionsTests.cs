using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse
{
    public class service_collection_extensions_AddBeatPulse_should
    {
        private readonly IServiceProvider _serviceProvider;

        public service_collection_extensions_AddBeatPulse_should()
        {
            _serviceProvider = WebHost.CreateDefaultBuilder()
                .UseStartup<DefaultStartup>()
                .Build()
                .Services;
        }


        [Fact]
        public void register_beat_pulse_service()
        {
            _serviceProvider.GetService<IBeatPulseService>()
                .Should()
                .BeOfType<BeatPulseService>();
        }

        [Fact]
        public void register_beat_pulse_contxt()
        {
            _serviceProvider.GetService<BeatPulseContext>()
                .Should()
                .BeOfType<BeatPulseContext>();
        }

        [Fact]
        public void set_valid_serviceprovider_to_context_to_allow_resolve_serviced_liveness()
        {
            var service = _serviceProvider.GetService<IBeatPulseService>(); // Need to resolve IBeatPulseService because its it who injects IServiceProvider to context
            var beatPulseContext = _serviceProvider.GetService<BeatPulseContext>(); 
            string path2;
            beatPulseContext.FindLiveness(nameof(path2))
                 .Should()
                 .NotBeNull();
        }

        [Fact]
        public void execute_the_context_setup()
        {
            var beatPulseContext = _serviceProvider.GetService<BeatPulseContext>();

            string path;

            beatPulseContext.FindLiveness(nameof(path))
                .Should()
                .NotBeNull();

        }

        class DefaultStartup
        {
            class FooService { }
            public void ConfigureServices(IServiceCollection services)
            {
                string name;
                string path;
                string path2;

                var taskResult = Task.FromResult((string.Empty, true));

                services.AddBeatPulse(context =>
                {
                    context.Add(new ActionLiveness(nameof(name), nameof(path), (httpcontext, cancellationToken) => taskResult));
                    context.Add(nameof(path2), sp =>
                    {
                        var service = sp.GetRequiredService<FooService>();
                        return new ActionLiveness(nameof(path2), nameof(path2), (httpcontext, cancellationToken) => taskResult);
                    });
                });

                services.AddTransient<FooService>();
            }

            public void Configure(IApplicationBuilder app)
            {
            }
        }
    }
}
