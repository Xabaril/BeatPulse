using System;
using System.Collections.Generic;
using System.Net.Http;
using BeatPulse.Uris;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Uri uri, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            var options = new UriLivenessOptions();
            options.AddUri(uri);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new UriLiveness(options));
        }

        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Uri uri, HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            var options = new UriLivenessOptions();
            options.AddUri(uri);
            options.UseHttpMethod(httpMethod);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new UriLiveness(options));
        }

        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, IEnumerable<Uri> uris, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            var options = UriLivenessOptions.CreateFromUris(uris);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new UriLiveness(options));
        }

        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, IEnumerable<Uri> uris, HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            var options = UriLivenessOptions.CreateFromUris(uris);
            options.UseHttpMethod(httpMethod);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new UriLiveness(options));
        }

        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Action<UriLivenessOptions> uriOptions, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            var options = new UriLivenessOptions();
            uriOptions?.Invoke(options);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new UriLiveness(options));
        }
    }
}
