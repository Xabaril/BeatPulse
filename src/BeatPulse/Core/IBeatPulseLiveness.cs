using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        Task<(string, bool)> IsHealthy(HttpContext context,LivenessContext livenessContext,CancellationToken cancellationToken = default);
    }
}
