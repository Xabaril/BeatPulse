using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public interface IUIAuthorizationFilter
    {
        Task<bool> Accept(HttpContext httpContext);
    }
}
