using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        Task<(string, bool)> IsHealthy(HttpContext context,LivenessExecutionContext livenessContext,CancellationToken cancellationToken = default);
    }
}
