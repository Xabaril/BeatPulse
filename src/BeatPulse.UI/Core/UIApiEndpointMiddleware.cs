using BeatPulse.UI.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class UIApiEndpointMiddleware
    {
        const string RESPONSE_CONTENT_TYPE = "application/json";

        private readonly RequestDelegate _next;

        public UIApiEndpointMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var runner = scope.ServiceProvider
                    .GetRequiredService<ILivenessRunner>();

                var cancellationToken = new CancellationToken();

                var registeredLiveness = await runner.GetLiveness(cancellationToken);

                var livenessRunsTasks = new List<Task<List<LivenessExecutionHistory>>>();

                foreach (var item in registeredLiveness)
                {
                    livenessRunsTasks.Add(runner.GetLatestRun(item.LivenessName, cancellationToken));
                }

                await Task.WhenAll(livenessRunsTasks);

                context.Response.ContentType = RESPONSE_CONTENT_TYPE;

                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(livenessRunsTasks.SelectMany(t => t.Result)));
            }
        }
    }
}
