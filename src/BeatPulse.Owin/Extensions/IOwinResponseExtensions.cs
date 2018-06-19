using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin.Extensions
{
    static class IOwinResponseExtensions
    {
        private static JsonSerializerSettings _defaultSerializationSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static Task WriteLivenessMessage(this IOwinResponse response, BeatPulseOptions options, OutputLivenessMessage message)
        {
            var defaultContentType = options.DetailedOutput ? "application/json" : "text/plain";

            const string noCacheOptions = "no-cache, no-store, must-revalidate";
            const string noCachePragma = "no-cache";
            const string defaultExpires = "0";

            response.Headers["Content-Type"] = defaultContentType;

            if (options.CacheOutput)
            {
                if (options.CacheMode == CacheMode.Header || options.CacheMode == CacheMode.HeaderAndServerMemory)
                {
                    response.Headers["Cache-Control"] = $"public, max-age={options.CacheDuration}";
                }
            }
            else
            {
                response.Headers["Cache-Control"] =  noCacheOptions;
                response.Headers["Pragma"] = noCachePragma;
                response.Headers["Expires"] = defaultExpires;
            }

            response.StatusCode = message.Code;

            var content = options.DetailedOutput ? JsonConvert.SerializeObject(message, _defaultSerializationSettings)
                : Enum.GetName(typeof(HttpStatusCode), message.Code);

            return response.WriteAsync(content);
        }
    }
}
