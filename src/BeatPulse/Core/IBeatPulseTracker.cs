using System.Threading.Tasks;

namespace BeatPulse.Core
{
    /// <summary>
    /// The BeatPulse service that track all health check results.
    /// </summary>
    public interface IBeatPulseTracker
    {
        /// <summary>
        /// Track liveness result.
        /// </summary>
        /// <param name="result">The <see cref="LivenessResult"/> to track.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task Track(LivenessResult result);
    }
}