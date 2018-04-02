using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BeatPulse.UI
{
    public class add_beat_pulseui_should
    {
        [Fact]
        public void register_mandatory_services()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var serviceCollection = new ServiceCollection();

            var services = BeatPulseServiceCollectionExtensions.AddBeatPulseUI(serviceCollection);
            services.AddSingleton<IConfiguration>(configurationBuilder.Build());

            var provider = services.BuildServiceProvider();

            provider.GetService<LivenessContext>()
                .Should().NotBeNull();

            provider.GetService<IHostedService>()
                .Should().NotBeNull();

            provider.GetService<ILivenessRunner>()
                .Should().NotBeNull();

            provider.GetService<ILivenessFailureNotifier>()
                .Should().NotBeNull();

        }
    }
}
