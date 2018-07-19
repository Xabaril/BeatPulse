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
                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(new System.Uri[] { uri }, HttpMethod.Get));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, Uri uri,HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(new Uri[] { uri }, httpMethod));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, IEnumerable<Uri> uris, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(uris, HttpMethod.Get));
            });
        }

        public static BeatPulseContext AddUrlGroup(this BeatPulseContext context, IEnumerable<Uri> uris, HttpMethod httpMethod, string defaultPath = "uri-group", string name = nameof(UriLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new UriLiveness(uris, httpMethod));
            });
        }
    }
}
