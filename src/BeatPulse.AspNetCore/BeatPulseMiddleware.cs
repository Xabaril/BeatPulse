using BeatPulse.Core;
using BeatPulse.Core.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeatPulse
{
    public class BeatPulseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BeatPulseOptions _options;
        private readonly IEnumerable<IBeatPulseAuthenticationFilter> _authenticationFilters;
        private readonly BeatPulseResponseCache _cache;

        public BeatPulseMiddleware(RequestDelegate next, IEnumerable<IBeatPulseAuthenticationFilter> authenticationFilters, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
            _authenticationFilters = authenticationFilters;
            _cache = new BeatPulseResponseCache(_options);
        }

        public async Task Invoke(HttpContext context, IBeatPulseService pulseService)
        {
            var request = context.Request;

            var beatPulsePath = context.GetBeatPulseRequestPath(_options);

            if (!await IsAuthenticatedRequest(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (_cache.TryGet(beatPulsePath, out OutputLivenessMessage output))
            {
                await request.HttpContext
                    .Response
                    .WriteLivenessMessage(_options, output);

                return;
            }

            output = new OutputLivenessMessage();

            var responses = await pulseService.IsHealthy(beatPulsePath, _options);

            if (!responses.Any())
            {
                // beat pulse path is not valid across any liveness
                // return unavailable with not found reason.
                output.SetNotFound();
            }
            else
            {
                // beat pulse is executed, set response
                // messages and add to cache if is configured.
                output.AddHealthCheckMessages(responses);
                output.SetExecuted();
                _cache.TryAddIfNeeded(beatPulsePath, output);
            }

            await request.HttpContext
               .Response
               .WriteLivenessMessage(_options, output);
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
