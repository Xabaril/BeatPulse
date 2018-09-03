using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default);
    }
}
