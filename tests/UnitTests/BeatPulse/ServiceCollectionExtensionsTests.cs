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
            public void ConfigureServices(IServiceCollection services)
            {
                string name;
                string path;

                var taskResult = Task.FromResult((string.Empty, true));

                services.AddBeatPulse(context =>
                {
                    context.Add(new ActionLiveness(nameof(name), nameof(path), httpContext => taskResult));
                });
            }

            public void Configure(IApplicationBuilder app)
            {
            }
        }
    }
}
