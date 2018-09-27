using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.Core
{
    public class action_liveness_should
    {
        [Fact]
        public async Task execute_definded_action_for_health_check()
        {
            var taskResult = Task.FromResult(LivenessResult.UnHealthy("action liveness is not working"));

            var livenessContext = new LivenessExecutionContext();

            var liveness = new ActionLiveness((cancellationToken) => taskResult);

            (await liveness.IsHealthy(livenessContext))
                    .Should().Be(taskResult.Result);
        }
    }
}
