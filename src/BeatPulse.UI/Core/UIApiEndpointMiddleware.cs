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
    class UIApiEndpointMiddleware
    {
        private readonly RequestDelegate _next;

        public UIApiEndpointMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<ILivenessRunner>();

                var cancellationToken = new CancellationToken();
                var registeredLiveness = await runner.GetLiveness(cancellationToken);

                var tasks = new List<Task<List<LivenessExecutionHistory>>>();

                foreach (var item in registeredLiveness)
                {
                    var livenessTask = runner.GetLatestRun(item.LivenessName, cancellationToken);
                    tasks.Add(livenessTask);
                }

                await Task.WhenAll(tasks);

                var responseContent = tasks.SelectMany(t => t.Result);

                context.Response.ContentType = Globals.DEFAULT_RESPONSE_CONTENT_TYPE;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseContent));
            }
        }
    }
}
