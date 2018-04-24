using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BeatPulse.Core.Authentication
{
    public interface IBeatPulseAuthenticationFilter
    {
        Task<bool> IsValid(HttpContext httpContext);
    }
}
