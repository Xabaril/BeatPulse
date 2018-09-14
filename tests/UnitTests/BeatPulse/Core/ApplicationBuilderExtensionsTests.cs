using BeatPulse;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class application_builder_extensions_should
    {
        [Fact]
        public void bind_beatpulse_settings_key_with_configured_configuration_provider_to_beatpulse_options()
        {
            var config = new Dictionary<string, string>()
            {
                { $"{BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY}:Path", "CustomPath"},
                { $"{BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY}:DetailedOutput", "true" },
                { $"{BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY}:CacheMode", "HeaderAndServerMemory" }
            };

            var configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(config)
                                .Build();

            var services = new ServiceCollection()
               .AddBeatPulse()
               .AddSingleton<IConfiguration>(c => configuration);

            var serviceProvider = services.BuildServiceProvider();

            var app = new ApplicationBuilder(serviceProvider);
            app.UseBeatPulse(setup =>
            {
                setup.Path.Should().Be("CustomPath");
                setup.DetailedOutput.Should().Be(true);
                setup.CacheMode.Should().Be((int)CacheMode.HeaderAndServerMemory);
            });
        }
    }
}
