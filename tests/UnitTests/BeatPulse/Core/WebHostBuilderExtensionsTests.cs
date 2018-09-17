using BeatPulse;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Xunit;
using Microsoft.Extensions.Configuration;
using UnitTests.Base;

namespace UnitTests.BeatPulse.Core
{
    public class webhostbuilder_extensions_should
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

            WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, configuration) =>
               {
                   configuration.AddInMemoryCollection(config);
               })
                 .UseBeatPulse(setup =>
                 {
                     setup.Path.Should().Be("CustomPath");
                     setup.DetailedOutput.Should().Be(true);
                     setup.CacheMode.Should().Be((int)CacheMode.HeaderAndServerMemory);
                 })
                 .UseStartup<DefaultStartup>()
                 .Build();
        }
    }
}
