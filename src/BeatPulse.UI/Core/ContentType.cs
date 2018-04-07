using System;
using System.Collections.Generic;

namespace BeatPulse.UI.Core
{
    class ContentType
    {
        public const string JAVASCRIPT = "text/javascript";
        public const string CSS = "text/css";
        public const string HTML = "text/html";
        public const string PLAIN = "text/plain";
        public const string PNG = "image/png";

        public static Dictionary<string, string> supportedContent =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"js", JAVASCRIPT },
            {"html", HTML },
            {"css", CSS },
            {"png", PNG }
        };

        public static string FromExtension(string fileExtension)
        {
            if (!supportedContent.ContainsKey(fileExtension))
            {
                return PLAIN;
            }
            return supportedContent[fileExtension];
        }
    }
}
