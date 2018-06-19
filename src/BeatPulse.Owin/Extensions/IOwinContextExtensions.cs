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
        public static string GetBeatPulseRequestPath(this IOwinContext context)
        {
            var path = context.Request.Path;
            return "";
        }
    }
}
