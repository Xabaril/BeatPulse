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
            var taskResult = Task.FromResult((string.Empty, false));

            var livenessContext = new LivenessExecutionContext();

            var liveness = new ActionLiveness(cancellationToken => taskResult);

            (await liveness.IsHealthy(livenessContext))
                    .Should().Be(taskResult.Result);
        }

    }
}
