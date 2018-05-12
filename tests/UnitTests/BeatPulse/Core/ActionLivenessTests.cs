using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.Core
{
    public class action_liveness_should
    {
        [Fact]
        public async Task execute_definded_action_for_health_check()
        {
            var taskResult = Task.FromResult((string.Empty,false));

            string defaultName;
            string defaultPath;

            var liveness = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                (context,cancellationToken) => taskResult);

            (await liveness.IsHealthy(new DefaultHttpContext(),isDevelopment:false))
                .Should().Be(taskResult.Result);
        }

        [Fact]
        public void get_specified_properties()
        {
            var taskResult = Task.FromResult((string.Empty, true));

            string defaultName;
            string defaultPath;

            var liveness = new ActionLiveness(
                nameof(defaultName),
                nameof(defaultPath),
                (httpcontext, cancellationToken) => taskResult);

            liveness.Name
                .Should().Be(nameof(defaultName));

            liveness.Path
                .Should().Be(nameof(defaultPath));
        }
    }
}
