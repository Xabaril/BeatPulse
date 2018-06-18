using BeatPulse.Core;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public class BeatPulseOwinMiddleware : OwinMiddleware
    {
        public BeatPulseOwinMiddleware(OwinMiddleware next, IBeatPulseService service) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.CompletedTask;
        }
    }
}
