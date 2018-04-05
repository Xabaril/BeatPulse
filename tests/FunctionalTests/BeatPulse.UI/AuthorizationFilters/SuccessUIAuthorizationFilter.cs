using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SuccessUIAuthorizationFilter : IUIAuthorizationFilter
{
    public Task<bool> Accept(HttpContext httpContext)
    {
        return Task.FromResult(true);
    }
}