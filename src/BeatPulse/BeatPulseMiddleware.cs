using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BeatPulse
{
    class BeatPulseMiddleware
    {

        class OutputMessage
        {
            private List<HealthCheckMessage> _messages;
            public IEnumerable<HealthCheckMessage> Checks => _messages;

            public DateTime StartedAtUtc { get; }

            public DateTime EndAtUtc { get; set; }

            public OutputMessage()
            {
                StartedAtUtc = DateTime.UtcNow;
                _messages = new List<HealthCheckMessage>();
            }

            public void AddHealthCheckMessages(IEnumerable<HealthCheckMessage> msgs) => _messages.AddRange(msgs);
        }

        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _templateMatcher;
        private readonly BeatPulseOptions _options;

        public BeatPulseMiddleware(RequestDelegate next, BeatPulseOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
            var requestPath = options.BeatPulsePath;

            _templateMatcher = new TemplateMatcher(TemplateParser.Parse($"{requestPath}/{{{BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME}}}"),
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
                var wantedOutput = _options.EnableOutput;
                var output = new OutputMessage();
                var checksResponses = await pulseService.IsHealthy(beatPulsePath, context);
                output.AddHealthCheckMessages(checksResponses);
                output.EndAtUtc = DateTime.UtcNow;

                await WriteResponseAsync(request.HttpContext, output, wantedOutput);
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

        Task WriteResponseAsync(HttpContext context, OutputMessage output, bool outputWanted)
        {
            const string defaultContentType = "application/json";
            const string defaultCacheOptions = "no-cache, no-store, must-revalidate";
            const string defaultPragma = "no-cache";
            const string defaultExpires = "0";

            context.Response.Headers["Content-Type"] = new[] { defaultContentType };
            context.Response.Headers["Cache-Control"] = new[] { defaultCacheOptions };
            context.Response.Headers["Pragma"] = new[] { defaultPragma };
            context.Response.Headers["Expires"] = new[] { defaultExpires };

            var statusCode = output.Checks.All(x => x.IsHealthy) ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
            context.Response.StatusCode = (int)statusCode;

            if (outputWanted)
            {
                var json = JsonConvert.SerializeObject(output);
                return context.Response.WriteAsync(json);
            }
            else
            {
                return context.Response.WriteAsync(Enum.GetName(typeof(HttpStatusCode), statusCode));
            }
        }
    }
}
