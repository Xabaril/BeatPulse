using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default);
    }
}
