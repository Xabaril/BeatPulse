using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core.Authentication
{
    class NoAuthenticationFilter : IBeatPulseAuthenticationFilter
    {
        public Task<bool> IsValid(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }
    }
}
