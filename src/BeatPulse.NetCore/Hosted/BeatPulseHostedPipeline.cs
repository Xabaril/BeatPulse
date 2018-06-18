using BeatPulse.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Hosted
{
    class BeatPulseHostedPipeline : IHostedService
    {
        private readonly IHostedBeatPulseEndpoint _endpoint;
        private readonly IBeatPulseService _beatpulseSvc;
        public BeatPulseHostedPipeline(IHostedBeatPulseEndpoint endpoint, IBeatPulseService beatpulseSvc)
        {
            _endpoint = endpoint;
            _beatpulseSvc = beatpulseSvc;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _endpoint.OnRequestReceivedAsync = ProcessRequest;
            return _endpoint.OpenAsync();
        }

        private async Task<bool> ProcessRequest()
        {
            var results = await _beatpulseSvc.IsHealthy("", new BeatPulseOptions());
            return results.All(x => x.IsHealthy);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _endpoint.Stop();
            return Task.CompletedTask;
        }
    }
}
