using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Threading.Tasks;

namespace BeatPulse
{
    class BeatPulseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _templateMatcher;

        public BeatPulseMiddleware(RequestDelegate next, string requestPath)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _templateMatcher = new TemplateMatcher(TemplateParser.Parse(requestPath),
                new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext context, IBeatPulseService pulseService)
        {
            var request = context.Request;

            if (!IsBeatPulseRequest(request))
            {
                await _next.Invoke(context);

                return;
            }
            else
            {
                //break circuit


            }

        }

        bool IsBeatPulseRequest(HttpRequest request)
        {
            return request.Method == HttpMethods.Get
                && _templateMatcher.TryMatch(request.Path, new RouteValueDictionary());
        }

        Task WriteResponseAsync(HttpContext context,string content,int statusCode)
        {
            const string defaultContentType = "application/json";
            const string defaultCacheOptions = "no-cache, no-store, must-revalidate";
            const string defaultPragma = "no-cache";
            const string defaultExpires = "0";

            context.Response.Headers["Content-Type"] = new[] { defaultContentType };
            context.Response.Headers["Cache-Control"] = new[] { defaultCacheOptions };
            context.Response.Headers["Pragma"] = new[] { defaultPragma };
            context.Response.Headers["Expires"] = new[] { defaultExpires };
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(content);
        }
    }
}
