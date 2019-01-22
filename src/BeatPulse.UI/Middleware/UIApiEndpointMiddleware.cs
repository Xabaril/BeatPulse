using BeatPulse.UI.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Middleware
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
                var db = scope.ServiceProvider.GetService<LivenessDb>();

                var cancellationToken = new CancellationToken();

                var registeredLiveness = await db.LivenessConfigurations
                    .ToListAsync(cancellationToken);

                var livenessExecutions = new List<LivenessExecution>();

                foreach (var item in registeredLiveness)
                {
                    var execution = await db.LivenessExecutions
                        .Include(le => le.History)
                        .Where(le => le.LivenessName.Equals(item.LivenessName, StringComparison.InvariantCultureIgnoreCase))
                        .AsNoTracking()
                        .SingleOrDefaultAsync(cancellationToken);

                    if (execution != null)
                    {
                        livenessExecutions.Add(execution);
                    }
                }

                var responseContent = JsonConvert.SerializeObject(livenessExecutions, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                context.Response.ContentType = BeatPulseUIKeys.DEFAULT_RESPONSE_CONTENT_TYPE;

                await context.Response.WriteAsync(responseContent);
            }
        }
    }
}
