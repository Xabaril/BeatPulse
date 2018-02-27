using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
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

        public BeatPulseMiddleware(RequestDelegate next, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;

            //match template for uri like /hc/{segment} 
            _templateMatcher = new TemplateMatcher(TemplateParser.Parse($"{options.BeatPulsePath}/{{{BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME}}}"),
                new RouteValueDictionary() { { BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME, string.Empty } });
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
                var output = new OutputMessage();

                //get responses from liveness

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var task = pulseService.IsHealthy(beatPulsePath, context,cancellationTokenSource.Token);

                    if (await Task.WhenAny(task, Task.Delay(_options.Timeout, cancellationTokenSource.Token)) == task)
                    {
                        var responses = await task;

                        output.AddHealthCheckMessages(responses);
                        output.EndAtUtc = DateTime.UtcNow;

                        await WriteResponseAsync(request.HttpContext, output, _options.DetailedOutput);
                    }
                    else
                    {
                        //timeout

                        cancellationTokenSource.Cancel();

                        await WriteTimeoutAsync(request.HttpContext);
                    }
                }
            }
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
            const string defaultContentType = "application/json";
            const string defaultCacheOptions = "no-cache, no-store, must-revalidate";
            const string defaultPragma = "no-cache";
            const string defaultExpires = "0";

            context.Response.Headers["Content-Type"] = new[] { defaultContentType };
            context.Response.Headers["Cache-Control"] = new[] { defaultCacheOptions };
            context.Response.Headers["Pragma"] = new[] { defaultPragma };
            context.Response.Headers["Expires"] = new[] { defaultExpires };

            var statusCode = message.Checks.All(x => x.IsHealthy) ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
            context.Response.StatusCode = (int)statusCode;

            var content = detailed ? JsonConvert.SerializeObject(message)
                : Enum.GetName(typeof(HttpStatusCode), statusCode);

            return context.Response.WriteAsync(content);
        }

        Task WriteTimeoutAsync(HttpContext context)
        {
            var statusCode = (int)HttpStatusCode.ServiceUnavailable;
            var content =  Enum.GetName(typeof(HttpStatusCode), statusCode);

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(content);
        }

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
