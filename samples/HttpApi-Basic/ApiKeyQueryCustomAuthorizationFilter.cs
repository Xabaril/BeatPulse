using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpApi_Basic
{
    public class ApiKeyQueryCustomAuthorizationFilter : IUIAuthorizationFilter
    {
        private readonly string _apiKey;

        public ApiKeyQueryCustomAuthorizationFilter(string apiKey)
        {
            _apiKey = apiKey;
        }
        public Task<bool> Accept(HttpContext httpContext)
        {
            var query = httpContext.Request.Query;
            const string queryKey = "apikey";

            return Task.FromResult(query.ContainsKey(queryKey) && query[queryKey].FirstOrDefault() == _apiKey);
        }
    }
}
