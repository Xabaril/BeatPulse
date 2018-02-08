using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class action_health_should
    {
        [Fact]
        public async Task execute_definded_action_for_health_check()
        {
            var result = false;

            var healthCheck = new ActionHealthCheck(
                "name",
                "defaultPath",
                (context) => result);

            (await healthCheck.IsHealthy(new DefaultHttpContext()))
                .Should().Be(result);
        }

        [Fact]
        public void get_specified_properties()
        {
            string defaultName;
            string defaultPath;

            var healthCheck = new ActionHealthCheck(
                nameof(defaultName),
                nameof(defaultPath),
                (context) => true);

            healthCheck.HealthCheckName
                .Should().Be(nameof(defaultName));

            healthCheck.HealthCheckDefaultPath
                .Should().Be(nameof(defaultPath));
        }
    }
}
