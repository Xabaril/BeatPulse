using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseHealthCheck
    {
        string HealthCheckName { get; }

        string HealthCheckDefaultPath { get; }

        IHealthCheckOptions Options { get; }

        Task<(string, bool)> IsHealthy(HttpContext context);
    }
}
