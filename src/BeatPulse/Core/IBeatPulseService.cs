using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseService
    {
        Task<IEnumerable<HealthCheckResult>> IsHealthy(string path, HttpContext context);
    }
}
