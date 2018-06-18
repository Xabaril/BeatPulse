using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Hosted
{
    class BeatPulseWebServer : IHostedBeatPulseEndpoint,  IDisposable
    {
        private readonly HttpListener _listener;
        private string _endpoint;

        public BeatPulseWebServer() => _listener = new HttpListener();

        public Func<Task<bool>> OnRequestReceivedAsync { get; set; }


        public Task OpenAsync()
        {
            _listener.Prefixes.Add("http://localhost:8080/hc/");
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
                var isOk = await OnRequestReceivedAsync();
                response.StatusCode = isOk ? 200 : 503;
                var data = Encoding.UTF8.GetBytes(isOk ? "OK" : "KO");
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

        public void Dispose() => ((IDisposable)_listener)?.Dispose();
    }
}
