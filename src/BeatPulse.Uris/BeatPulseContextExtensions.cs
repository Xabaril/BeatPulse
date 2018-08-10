using BeatPulse.Core;
using BeatPulse.Uris;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, Uri uri, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                var options = new UriLivenessOptions();
                options.AddUri(uri);

                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(options));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, Uri uri, HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                var options = new UriLivenessOptions();
                options.AddUri(uri);
                options.UseHttpMethod(httpMethod);

                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(options));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, IEnumerable<Uri> uris, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                var options = UriLivenessOptions.CreateFromUris(uris);

                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(options));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, IEnumerable<Uri> uris, HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                var options = UriLivenessOptions.CreateFromUris(uris);
                options.UseHttpMethod(httpMethod);

                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(options));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, Action<UriLivenessOptions> uriOptions, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                var options = new UriLivenessOptions();
                uriOptions.Invoke(options);

                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(options));
            });

        }

    }
}
