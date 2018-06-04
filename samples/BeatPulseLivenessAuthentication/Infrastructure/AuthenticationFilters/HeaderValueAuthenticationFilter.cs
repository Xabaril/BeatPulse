using BeatPulse.Core.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BeatPulseLivenessAuthentication.Infrastructure.AuthenticationFilters
{
    public class HeaderValueAuthenticationFilter : IBeatPulseAuthenticationFilter
    {
        private readonly string _headerName;
        private readonly string _headerValue;

        public HeaderValueAuthenticationFilter(string headerName, string headerValue)
        {
            _headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
            _headerValue = headerValue ?? throw new ArgumentNullException(nameof(headerValue));
        }
        public Task<bool> IsValid(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers[_headerName];

            if (String.Equals(header, _headerValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
