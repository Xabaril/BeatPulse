using BeatPulse.UI.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class UIApiEndpointMiddleware
    {
        private readonly RequestDelegate _next;

        public UIApiEndpointMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
        {
            using(var scope = serviceScopeFactory.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<ILivenessRunner>();
                var cancelToken = new CancellationToken();                
                var registeredLiveness = await runner.GetLiveness(cancelToken);

                List<Task<List<LivenessExecutionHistory>>> livenessRunsTasks = new List<Task<List<LivenessExecutionHistory>>>();

                foreach (var liveness in registeredLiveness)
                {
                    livenessRunsTasks.Add(runner.GetLatestRun(liveness.LivenessName, cancelToken));
                }

                await Task.WhenAll(livenessRunsTasks);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(livenessRunsTasks.SelectMany(t => t.Result))
                );
            }
        }
    }
}
