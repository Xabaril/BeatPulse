using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    /// <summary>
    /// The BeatPulse Liveness contract
    /// </summary>
    public interface IBeatPulseLiveness
    {
        /// <summary>
        /// Check if the service to check is healthy or not.
        /// </summary>
        /// <param name="context">The <see cref="LivenessExecutionContext"/> to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The health check status result.</returns>
        Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default);
    }
}
