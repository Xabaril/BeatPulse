using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        string Name { get; }

        string Path { get; }

        Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment,CancellationToken cancellationToken = default);
    }
}
