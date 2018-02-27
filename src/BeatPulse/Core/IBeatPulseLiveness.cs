using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseLiveness
    {
        string Name { get; }

        string DefaultPath { get; }

        Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment);
    }
}
