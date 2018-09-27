using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    /// <summary>
    /// The BeatPulse service that check all configured liveness.
    /// </summary>
    public interface IBeatPulseService
    {
        /// <summary>
        /// Get the health check status from all configured liveness.
        /// </summary>
        /// <param name="path">The path on with liveness are configured.</param>
        /// <param name="options">The <see cref="BeatPulseOptions"/> to be used.</param>
        /// <param name="context">The current <see cref="HttpContext"/.></param>
        /// <returns>All results from liveness configured on <paramref name="path"/>.</returns>
        Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options, HttpContext context);
    }
}
