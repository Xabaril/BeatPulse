using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing.Template;

namespace UnitTests.BeatPulse
{
    public class service_collection_extensions_AddBeatPulse_should
    {
        private readonly ServiceCollection _defaultServiceCollection;

        public service_collection_extensions_AddBeatPulse_should()
        {
            _defaultServiceCollection = new ServiceCollection();

            var loggerFactory = new LoggerFactory();

            _defaultServiceCollection.AddLogging(builder =>
            {
                builder.AddDebug();
            });
        }

       
        [Fact]
        public void register_beat_pulse_service()
        {
            _defaultServiceCollection.AddBeatPulse(context =>
            {
            });

            _defaultServiceCollection.BuildServiceProvider()
                .GetService<IBeatPulseService>()
                .Should()
                .BeOfType<BeatPulseService>();
        }

        [Fact]
        public void register_beat_pulse_contxt()
        {
            _defaultServiceCollection.AddBeatPulse(context =>
            {
            });

            _defaultServiceCollection.BuildServiceProvider()
                .GetService<BeatPulseContext>()
                .Should()
                .BeOfType<BeatPulseContext>();
        }

        [Fact]
        public void execute_the_context_setup()
        {
            string name;
            string path;
            
            _defaultServiceCollection.AddBeatPulse(context =>
            {
                context.Add(new ActionHealthCheck(nameof(name), nameof(path), httpContext => true));
            });

            var beatPulseContext = _defaultServiceCollection.BuildServiceProvider()
                .GetService<BeatPulseContext>();

            beatPulseContext.Find(nameof(path))
                .Should()
                .NotBeNull();

        }
    }
}
