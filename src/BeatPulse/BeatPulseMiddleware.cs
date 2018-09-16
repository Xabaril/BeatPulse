using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeatPulse.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse
{
    public class BeatPulseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BeatPulseOptions _options;
        private readonly IEnumerable<IBeatPulseAuthenticationFilter> _authenticationFilters;
        private readonly ConcurrentDictionary<string, OutputLivenessMessage> _cache;

        public BeatPulseMiddleware(RequestDelegate next, IEnumerable<IBeatPulseAuthenticationFilter> authenticationFilters, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
            _authenticationFilters = authenticationFilters;
            _cache = new ConcurrentDictionary<string, OutputLivenessMessage>();
        }

        public async Task Invoke(HttpContext context, HealthCheckService healthCheckService)
        {
            var request = context.Request;

            var beatPulsePath = context.GetBeatPulseRequestPath(_options);

            if (!await IsAuthenticatedRequest(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (TryFromCache(beatPulsePath, out OutputLivenessMessage output))
            {
                await request.HttpContext
                    .Response
                    .WriteLivenessMessage(_options, output);

                return;
            }

            output = new OutputLivenessMessage();

            var report = await healthCheckService.CheckHealthAsync((registration) => 
            {
                registration.Tags.Contains(beatPulsePath);
                return true;
            });

            if (!report.Entries.Any())
            {
                // beat pulse path is not valid across any liveness
                // return unavailable with not found reason.
                output.SetNotFound();
            }
            else
            {
                // beat pulse is executed, set response
                // messages and add to cache if is configured.
                output.AddHealthCheckMessages(report.Entries.Values);
                output.SetExecuted();

                if (_options.CacheMode.UseServerMemory() && _options.CacheOutput)
                {
                    _cache.TryAdd(beatPulsePath, output);
                }
            }

            await request.HttpContext
               .Response
               .WriteLivenessMessage(_options, output);
        }

        bool TryFromCache(string path, out OutputLivenessMessage message)
        {
            message = null;

            if (_options.CacheOutput
                && _options.CacheMode.UseServerMemory()
                && _cache.TryGetValue(path, out message))
            {
                var seconds = (DateTime.UtcNow - message.EndAtUtc).TotalSeconds;

                if (_options.CacheDuration > seconds)
                {
                    return true;
                }
                else
                {
                    _cache.TryRemove(path, out OutputLivenessMessage removed);

                    return false;
                }
            }

            return false;
        }

        async Task<bool> IsAuthenticatedRequest(HttpContext httpContext)
        {
            foreach (var filter in _authenticationFilters)
            {
                if (!await filter.IsValid(httpContext))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
