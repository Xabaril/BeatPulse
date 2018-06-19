using BeatPulse.Core;
using BeatPulse.Owin.Extensions;
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
        private readonly IBeatPulseService _service;
        private readonly BeatPulseOptions _options;
        public BeatPulseOwinMiddleware(OwinMiddleware next, IBeatPulseService service, BeatPulseOptions options) : base(next)
        {
            _service = service;
            _options = options;
        }

        public async override Task Invoke(IOwinContext context)
        {
            var request = context.Request;
            var beatPulsePath = context.GetBeatPulseRequestPath();
        }
    }
}
