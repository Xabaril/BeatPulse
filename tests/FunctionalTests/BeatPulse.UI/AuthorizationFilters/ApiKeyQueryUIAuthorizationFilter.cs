using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

public class ApiKeyQueryUIAuthorizationFilter : IUIAuthorizationFilter
{
    private readonly string _apiKey;

    public ApiKeyQueryUIAuthorizationFilter(string apiKey)
    {
        _apiKey = apiKey;
    }
    public Task<bool> Accept(HttpContext httpContext)
    {
        var query = httpContext.Request.Query;
        const string queryKey = "apikey";

        var keyValue = query[queryKey].FirstOrDefault();
        return Task.FromResult(query.ContainsKey(queryKey) &&  keyValue == _apiKey);
    }
}