using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BeatPulse.Core.Authentication
{
    public class ApiKeyAuthenticationFilter
        : IBeatPulseAuthenticationFilter
    {
        private readonly string _apiKeyValue;
        private readonly string _apiKeyName;

        public ApiKeyAuthenticationFilter(string apiKeyValue, string apiKeyName = "api-key")
        {
            _apiKeyValue = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));
            _apiKeyName = apiKeyName ?? throw new ArgumentNullException(nameof(apiKeyName));
        }

        public Task<bool> IsValid(HttpContext httpContext)
        {
            var apiKeyValues = httpContext.Request?.Query[_apiKeyName];

            if ( apiKeyValues.HasValue )
            {
                foreach (var item in apiKeyValues.Value)
                {
                    if (String.Equals(item, _apiKeyValue, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }
    }
}
