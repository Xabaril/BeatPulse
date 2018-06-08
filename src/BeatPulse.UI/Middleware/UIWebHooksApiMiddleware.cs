using BeatPulse.UI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class UIWebHooksApiMiddleware
    {       
        private readonly JsonSerializerSettings _jsonSerializationSettings;        
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UIWebHooksApiMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {            
            _jsonSerializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var beatPulseSettings = scope.ServiceProvider.GetService<IOptions<BeatPulseSettings>>();                
                context.Response.ContentType = BeatPulseUIKeys.DEFAULT_RESPONSE_CONTENT_TYPE;

                var sanitizedWebhooks = beatPulseSettings.Value.Webhooks.Select(w =>
                {
                    dynamic webhook = new {
                                        w.Name,
                                        w.Uri,
                                        Payload = JObject.Parse(Regex.Unescape(w.Payload))
                                     };
                    return webhook;
                });
                
                await context.Response.WriteAsync(JsonConvert.SerializeObject(sanitizedWebhooks, _jsonSerializationSettings));
            }            
        } 
    }
}
