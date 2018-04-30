using BeatPulse.Core;
using BeatPulse.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse
{
    class BeatPulseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _templateMatcher;
        private readonly BeatPulseOptions _options;
        private readonly IEnumerable<IBeatPulseAuthenticationFilter> _authenticationFilters;
        private readonly ConcurrentDictionary<string, OutputMessage> _cache;

        public BeatPulseMiddleware(RequestDelegate next, IEnumerable<IBeatPulseAuthenticationFilter> authenticationFilters, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
            _authenticationFilters = authenticationFilters;
            _cache = new ConcurrentDictionary<string, OutputMessage>();
            _templateMatcher = new TemplateMatcher(TemplateParser.Parse($"{options.BeatPulsePath}/{{{BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME}}}"),
                new RouteValueDictionary() { { BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME, string.Empty } });//match template for uri like /hc/{segment} 
        }

        public async Task Invoke(HttpContext context, IBeatPulseService pulseService)
        {
            var request = context.Request;

            if (!IsBeatPulseRequest(request, out string beatPulsePath))
            {
                await _next.Invoke(context);
                return;
            }

            if (!await IsAuthenticatedRequest(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (TryFromCache(beatPulsePath, out OutputMessage output))
            {
                await WriteResponseAsync(request.HttpContext, output);
                return;
            }

            output = new OutputMessage();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var task = pulseService.IsHealthy(beatPulsePath, context, cancellationTokenSource.Token);

                if (await Task.WhenAny(task, Task.Delay(_options.Timeout, cancellationTokenSource.Token)) == task)
                {
                    var responses = await task;

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

                        if (_options.CacheMode.UseServerMemory() && _options.CacheOutput)
                        {
                            _cache.TryAdd(beatPulsePath, output);
                        }
                    }
                }
                else
                {
                    // beat pulse services is timeout, because is configured using BeatPulseOptions on UseBeatPulse method.
                    // In this case remove from cache if exist and return a ServiceUnavailabe with timeout response resason.
                    _cache.TryRemove(beatPulsePath, out OutputMessage removed);
                    cancellationTokenSource.Cancel();
                    output.SetTimeout();
                }

                await WriteResponseAsync(request.HttpContext, output);
            }
        }

        bool TryFromCache(string path, out OutputMessage message)
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
                    _cache.TryRemove(path, out OutputMessage removed);

                    return false;
                }
            }

            return false;
        }

        bool IsBeatPulseRequest(HttpRequest request, out string beatPulsePath)
        {
            beatPulsePath = string.Empty;

            var routeValues = new RouteValueDictionary();

            var isValidRequest = request.Method == HttpMethods.Get
                && _templateMatcher.TryMatch(request.Path, routeValues);

            if (isValidRequest)
            {
                beatPulsePath = routeValues[BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME]
                    .ToString();
            }

            return isValidRequest;
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

        Task WriteResponseAsync(HttpContext httpContext, OutputMessage message)
        {
            var defaultContentType = _options.DetailedOutput ? "application/json" : "text/plain";

            const string noCacheOptions = "no-cache, no-store, must-revalidate";
            const string noCachePragma = "no-cache";
            const string defaultExpires = "0";

            httpContext.Response.Headers["Content-Type"] = new[] { defaultContentType };

            if (_options.CacheOutput)
            {
                if (_options.CacheMode == CacheMode.Header || _options.CacheMode == CacheMode.HeaderAndServerMemory)
                {
                    httpContext.Response.Headers["Cache-Control"] = new[] { $"public, max-age={_options.CacheDuration}" };
                }
            }
            else
            {
                httpContext.Response.Headers["Cache-Control"] = new[] { noCacheOptions };
                httpContext.Response.Headers["Pragma"] = new[] { noCachePragma };
                httpContext.Response.Headers["Expires"] = new[] { defaultExpires };
            }

            httpContext.Response.StatusCode = message.Code;

            var content = _options.DetailedOutput ? JsonConvert.SerializeObject(message)
                : Enum.GetName(typeof(HttpStatusCode), message.Code);

            return httpContext.Response.WriteAsync(content);
        }

        private class OutputMessage
        {
            const string INVALID_BEATPULSE_PATH = "InvalidBeatPulsePath";
            const string BEATPULSE_TIMEOUT = "BeatPulseTimeout";

            private readonly List<LivenessResult> _messages = new List<LivenessResult>();

            public IEnumerable<LivenessResult> Checks => _messages;

            public DateTime StartedAtUtc { get; private set; }

            public DateTime EndAtUtc { get; private set; }

            public int Code { get; private set; }

            public string Reason { get; private set; }

            public OutputMessage()
            {
                StartedAtUtc = DateTime.UtcNow;
            }

            public void AddHealthCheckMessages(IEnumerable<LivenessResult> messages) => _messages.AddRange(messages);

            public void SetTimeout()
            {
                EndAtUtc = DateTime.UtcNow;
                Code = StatusCodes.Status503ServiceUnavailable;
                Reason = BEATPULSE_TIMEOUT;
            }

            public void SetNotFound()
            {
                EndAtUtc = DateTime.UtcNow;
                Code = StatusCodes.Status503ServiceUnavailable;
                Reason = INVALID_BEATPULSE_PATH;
            }

            public void SetExecuted()
            {
                EndAtUtc = DateTime.UtcNow;
                Code = Checks.All(x => x.IsHealthy) ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            }
        }
    }
}
