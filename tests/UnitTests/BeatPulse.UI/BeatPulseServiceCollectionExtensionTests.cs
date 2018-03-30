using BeatPulse.UI;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.HostedService;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BeatPulse.UI
{
    public class add_beat_pulseui_should
    {
        [Fact]
        public void register_mandatory_services()
        {
            var serviceCollection = new ServiceCollection();

            var services = BeatPulseServiceCollectionExtensions.AddBeatPulseUI(serviceCollection);

            var provider = services.BuildServiceProvider();

            provider.GetService<LivenessContext>()
                .Should().NotBeNull();

            provider.GetService<LivenessHostedService>()
                .Should().NotBeNull();
        }
    }
}
