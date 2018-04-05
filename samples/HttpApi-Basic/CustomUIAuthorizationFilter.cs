using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpApi_Basic
{
    public class CustomUIAuthorizationFilter : IUIAuthorizationFilter
    {
        public Task<bool> Accept(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }
    }
}
