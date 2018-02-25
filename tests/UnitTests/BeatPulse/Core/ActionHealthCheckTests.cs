using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.Core
{
    public class action_health_should
    {
        [Fact]
        public async Task execute_definded_action_for_health_check()
        {
            var taskResult = Task.FromResult((string.Empty,false));

            string defaultName;
            string defaultPath;

            var healthCheck = new ActionHealthCheck(
                nameof(defaultName),
                nameof(defaultPath),
                (context) => taskResult);

            (await healthCheck.IsHealthy(new DefaultHttpContext(),isDevelopment:false))
                .Should().Be(taskResult.Result);
        }

        [Fact]
        public void get_specified_properties()
        {
            var taskResult = Task.FromResult((string.Empty, true));

            string defaultName;
            string defaultPath;
            

            var healthCheck = new ActionHealthCheck(
                nameof(defaultName),
                nameof(defaultPath),
                (context) => taskResult);

            healthCheck.HealthCheckName
                .Should().Be(nameof(defaultName));

            healthCheck.HealthCheckDefaultPath
                .Should().Be(nameof(defaultPath));
        }
    }
}
