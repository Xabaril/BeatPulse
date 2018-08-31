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
        private readonly BeatPulseHostedOptions _hostedOptions;
        private readonly BeatPulseOptions _options;
        public BeatPulseHostedPipeline(IHostedBeatPulseEndpoint endpoint, IBeatPulseService beatpulseSvc, BeatPulseHostedOptions hostedOptions)
        {
            _endpoint = endpoint;
            _beatpulseSvc = beatpulseSvc;
            _hostedOptions = hostedOptions;
            _options = hostedOptions.BuildBeatPulseOptions();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _endpoint.Setup(_hostedOptions);
            _endpoint.OnRequestReceivedAsync = ProcessRequest;
            return _endpoint.OpenAsync();
        }

        private async Task<IEnumerable<LivenessResult>> ProcessRequest(string path)
        {
            var results = await _beatpulseSvc.IsHealthy(path, _options);
            return results;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _endpoint.Stop();
            return Task.CompletedTask;
        }
    }
}
