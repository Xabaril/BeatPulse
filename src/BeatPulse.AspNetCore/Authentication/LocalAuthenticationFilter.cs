using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace BeatPulse.Core.Authentication
{
    public class LocalAuthenticationFilter
        : IBeatPulseAuthenticationFilter
    {
        public Task<bool> IsValid(HttpContext httpContext)
        {
            if (IPAddress.IsLoopback(httpContext.Connection.RemoteIpAddress))
            {
                return Task.FromResult(true);
            }

            if (httpContext.Connection.RemoteIpAddress.Equals(httpContext.Connection.LocalIpAddress))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
