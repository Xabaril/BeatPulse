using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseService
    {
        Task<bool> IsHealthy(string path, HttpContext context);
    }
}
