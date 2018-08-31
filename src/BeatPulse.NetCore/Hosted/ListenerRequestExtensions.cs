using System;
using System.Net;

namespace BeatPulse.NetCore.Hosted
{
    static class ListenerRequestExtensions
    {
        public static string GetBeatpulsePath(this HttpListenerRequest req)
        {
            var uri = req.Url;
            var fullPath = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            var paths = fullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return paths.Length > 1 ? paths[paths.Length - 1] : "";
        }
    }
}
