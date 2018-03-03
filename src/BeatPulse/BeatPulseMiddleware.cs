using BeatPulse.Core;
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
        private readonly ConcurrentDictionary<string, OutputMessage> _cache;

        public BeatPulseMiddleware(RequestDelegate next, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
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
            else
            {
                if (TryFromCache(beatPulsePath, out OutputMessage output))
                {
                    await WriteResponseAsync(request.HttpContext, output, _options.DetailedOutput);
                }
                else
                {
                    output = new OutputMessage();

                    using (var cancellationTokenSource = new CancellationTokenSource())
                    {
                        var task = pulseService.IsHealthy(beatPulsePath, context, cancellationTokenSource.Token);

                        if (await Task.WhenAny(task, Task.Delay(_options.Timeout, cancellationTokenSource.Token)) == task)
                        {
                            var responses = await task;

                            if (!responses.Any())
                            {
                                await WriteNotFoundAsync(request.HttpContext, _options.DetailedOutput);

                                return;
                            }

                            output.AddHealthCheckMessages(responses);
                            output.EndAtUtc = DateTime.UtcNow;

                            if (_options.CacheMode.UseServerMemory() && _options.CacheOutput)
                            {
                                _cache.TryAdd(beatPulsePath, output);
                            }

                            await WriteResponseAsync(request.HttpContext, output, _options.DetailedOutput);
                        }
                        else
                        {
                            //timeout
                            _cache.TryRemove(beatPulsePath, out OutputMessage removed);

                            cancellationTokenSource.Cancel();

                            await WriteTimeoutAsync(request.HttpContext, _options.DetailedOutput);
                        }
                    }
                }
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

        Task WriteResponseAsync(HttpContext context, OutputMessage message, bool detailed)
        {
            var defaultContentType = detailed ? "application/json" : "text/plain";
            const string noCacheOptions = "no-cache, no-store, must-revalidate";
            const string noCachePragma = "no-cache";
            const string defaultExpires = "0";

            context.Response.Headers["Content-Type"] = new[] { defaultContentType };

            if (_options.CacheOutput)
            {
                if (_options.CacheMode == CacheMode.Header || _options.CacheMode == CacheMode.HeaderAndServerMemory)
                {
                    context.Response.Headers["Cache-Control"] = new[] { $"public, max-age={_options.CacheDuration}" };
                }
            }
            else
            {
                context.Response.Headers["Cache-Control"] = new[] { noCacheOptions };
                context.Response.Headers["Pragma"] = new[] { noCachePragma };
                context.Response.Headers["Expires"] = new[] { defaultExpires };

            }

            var statusCode = message.Checks.All(x => x.IsHealthy) ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
            context.Response.StatusCode = (int)statusCode;

            var content = detailed ? JsonConvert.SerializeObject(message)
                : Enum.GetName(typeof(HttpStatusCode), statusCode);

            return context.Response.WriteAsync(content);
        }
        Task WriteStatusCodeAsync(HttpContext context, bool detailed, HttpStatusCode httpcode, string reason = null)
        {
            var defaultContentType = detailed ? "application/json" : "text/plain";
            var code = Enum.GetName(typeof(HttpStatusCode), httpcode);

            context.Response.Headers["Content-Type"] = new[] { defaultContentType };
            context.Response.StatusCode = (int)httpcode;

            var content = detailed ? JsonConvert.SerializeObject(new { Code = code, Reason = reason })
                : code;

            return context.Response.WriteAsync(content);
        }

        Task WriteTimeoutAsync(HttpContext context, bool detailed) => WriteStatusCodeAsync(context, detailed, HttpStatusCode.ServiceUnavailable, "Timeout");

        Task WriteNotFoundAsync(HttpContext context, bool detailed) => WriteStatusCodeAsync(context, detailed, HttpStatusCode.NotFound, "Beatpulse Path not found");

        private class OutputMessage
        {
            private readonly List<LivenessResult> _messages = new List<LivenessResult>();

            public IEnumerable<LivenessResult> Checks => _messages;

            public DateTime StartedAtUtc { get; }

            public DateTime EndAtUtc { get; set; }

            public OutputMessage()
            {
                StartedAtUtc = DateTime.UtcNow;
            }

            public void AddHealthCheckMessages(IEnumerable<LivenessResult> messages) => _messages.AddRange(messages);
        }
    }
}
