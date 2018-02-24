using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseHealthCheck
    {
        string HealthCheckName { get; }

        string HealthCheckDefaultPath { get; }

        Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment);
    }
}
