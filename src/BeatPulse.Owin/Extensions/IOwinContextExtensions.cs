using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin.Extensions
{
    static class IOwinContextExtensions
    {
        public static string GetBeatPulseRequestPath(this IOwinContext context, BeatPulseOptions options)
        {
            var path = context.Request.Path;
            var root = new PathString(options.BeatPulsePath);
            var starts = path.StartsWithSegments(root, out PathString rest);
            return starts ? rest.ToUriComponent() : null;
        }
    }
}
