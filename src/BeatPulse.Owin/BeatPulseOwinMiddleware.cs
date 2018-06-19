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
        private readonly BeatPulseResponseCache _cache;
        public BeatPulseOwinMiddleware(OwinMiddleware next, IBeatPulseService service, BeatPulseOptions options) : base(next)
        {
            _service = service;
            _options = options;
            _cache = new BeatPulseResponseCache(_options);
        }

        public async override Task Invoke(IOwinContext context)
        {
            var request = context.Request;
            var beatPulsePath = context.GetBeatPulseRequestPath(_options);
            if (beatPulsePath != null)
            {
                await ProcessRequest(context, beatPulsePath);
            }
        }

        private async Task ProcessRequest(IOwinContext context, string beatPulsePath)
        {
            if (_cache.TryGet(beatPulsePath, out OutputLivenessMessage output))
            {
                await context.Response.WriteLivenessMessage(_options, output);
                return;
            }
            output = new OutputLivenessMessage();
            var responses = await _service.IsHealthy(beatPulsePath, _options);
            if (!responses.Any())
            {
                output.SetNotFound();
            }
            else
            {
                output.AddHealthCheckMessages(responses);
                output.SetExecuted();
                _cache.TryAddIfNeeded(beatPulsePath, output);
            }

            await context.Response.WriteLivenessMessage(_options, output);
        }
    }
}
