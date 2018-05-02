using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BeatPulse
{
    static class HttpResponseExtensions
    {
        public static Task WriteLivenessMessage(this HttpResponse response,BeatPulseOptions options, OutputLivenessMessage message)
        {
            var defaultContentType = options.DetailedOutput ? "application/json" : "text/plain";

            const string noCacheOptions = "no-cache, no-store, must-revalidate";
            const string noCachePragma = "no-cache";
            const string defaultExpires = "0";

            response.Headers["Content-Type"] = new[] { defaultContentType };

            if (options.CacheOutput)
            {
                if (options.CacheMode == CacheMode.Header || options.CacheMode == CacheMode.HeaderAndServerMemory)
                {
                    response.Headers["Cache-Control"] = new[] { $"public, max-age={options.CacheDuration}" };
                }
            }
            else
            {
                response.Headers["Cache-Control"] = new[] { noCacheOptions };
                response.Headers["Pragma"] = new[] { noCachePragma };
                response.Headers["Expires"] = new[] { defaultExpires };
            }

            response.StatusCode = message.Code;

            var content = options.DetailedOutput ? JsonConvert.SerializeObject(message)
                : Enum.GetName(typeof(HttpStatusCode), message.Code);

            return response.WriteAsync(content);
        }
    }
}
