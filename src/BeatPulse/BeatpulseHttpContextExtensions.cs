using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public static class BeatpulseHttpContextExtensions
    {
        public static bool IsBeatPulseRequest(this HttpContext context, BeatPulseOptions options)
        {
            return context.Request.Method == HttpMethods.Get
                && context.MatchesBeatPulseRequestPath(options);
        }

        public static string GetBeatPulseRequestPath(this HttpContext context, BeatPulseOptions options)
        {
            var routeValues = new RouteValueDictionary();
            var templateMatcher = GetTemplateMatcher(options);
            
            templateMatcher.TryMatch(context.Request.Path, routeValues);
            return routeValues[BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME].ToString();
        }

        private static bool MatchesBeatPulseRequestPath(this HttpContext context, BeatPulseOptions options)
        {
            var routeValues = new RouteValueDictionary();
            var templateMatcher = GetTemplateMatcher(options);

            return templateMatcher.TryMatch(context.Request.Path, routeValues);
        }

        private static TemplateMatcher GetTemplateMatcher(BeatPulseOptions options)
        {
            var templateMatcher = new TemplateMatcher(TemplateParser.Parse($"{options.BeatPulsePath}/{{{BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME}}}"),
            new RouteValueDictionary() { { BeatPulseKeys.BEATPULSE_PATH_SEGMENT_NAME, string.Empty } });

            return templateMatcher;
        }
    }
}
