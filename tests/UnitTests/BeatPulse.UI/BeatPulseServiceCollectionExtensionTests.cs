using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.Notifications;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());

            var services = BeatPulseServiceCollectionExtensions.AddBeatPulseUI(serviceCollection);
           

            var provider = services.BuildServiceProvider();

            provider.GetService<LivenessDb>()
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
