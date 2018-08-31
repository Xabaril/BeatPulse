using BeatPulse.Core;
using BeatPulse.NetCore.Hosted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Hosted
{
    class BeatPulseWebServer : IHostedBeatPulseEndpoint, IDisposable
    {
        private readonly HttpListener _listener;
        private readonly string _endpoint;
        private BeatPulseHostedOptions _options;
        private BeatPulseOptions _beatPulseOptions;
        private bool _initialized;
        private readonly IHostedBeatpulseOutputFormatter _formatter;

        public BeatPulseWebServer(IHostedBeatpulseOutputFormatter outputFormatter)
        {
            _listener = new HttpListener();
            _initialized = false;
            _formatter = outputFormatter;
        }

        public Func<string, Task<IEnumerable<LivenessResult>>> OnRequestReceivedAsync { get; set; }

        public void Setup(BeatPulseHostedOptions options)
        {
            _options = options;
            _beatPulseOptions = options.BuildBeatPulseOptions();
            _initialized = true;
        }


        public Task OpenAsync()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException($"Must call {nameof(Setup)} first");
            }
            _listener.Prefixes.Add($"http://localhost:{_options.Port}/{_options.Path}/");
            return Task.Factory.StartNew(ReceiveMessages);
        }


        private async Task ReceiveMessages()
        {
            _listener.Start();
            while (_listener.IsListening)
            {
                var ctx = await _listener.GetContextAsync();
                var req = ctx.Request;
                var response = ctx.Response;
                var beatPulsePath = req.GetBeatpulsePath();
                var outputMessage = new OutputLivenessMessage();
                var results = await OnRequestReceivedAsync(beatPulsePath);
                outputMessage.AddHealthCheckMessages(results);
                outputMessage.SetExecuted();
                response.StatusCode = outputMessage.Code;
                var data = _formatter.Serialize(outputMessage, _beatPulseOptions);
                response.ContentLength64 = data.Length;
                var output = response.OutputStream;
                await output.WriteAsync(data, 0, data.Length);
                output.Close();
            }

        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public void Dispose()
        {
            ((IDisposable)_listener)?.Dispose();
            _initialized = false;
        }
    }
}
