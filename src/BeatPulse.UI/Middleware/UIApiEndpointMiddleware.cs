using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Middleware
{
    class UIApiEndpointMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _jsonSerializationSettings;

        public UIApiEndpointMiddleware(RequestDelegate next)
        {
            _next = next;

            _jsonSerializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<ILivenessRunner>();

                var cancellationToken = new CancellationToken();

                var registeredLiveness = await runner.GetConfiguredLiveness(cancellationToken);

                var tasks = new List<Task<LivenessExecution>>();

                foreach (var item in registeredLiveness)
                {
                    var livenessTask = runner.GetLatestRun(item.LivenessName, cancellationToken);
                    tasks.Add(livenessTask);
                }

                await Task.WhenAll(tasks);

                var livenessResult = tasks.Select(t => t.Result);
                var responseContent = JsonConvert.SerializeObject(livenessResult, _jsonSerializationSettings);

                context.Response.ContentType = BeatPulseUIKeys.DEFAULT_RESPONSE_CONTENT_TYPE;

                await context.Response.WriteAsync(responseContent);
            }
        }
    }
}
