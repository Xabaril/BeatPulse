using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BeatPulse.UI.Core.Builders
{
    class FakeLivenessRunner : LivenessRunner
    {
        private readonly HttpStatusCode _status;
        private readonly string _content;

        public FakeLivenessRunner(HttpStatusCode httpStatus,string content,
            LivenessDb context, 
            ILivenessFailureNotifier notifier,
            ILogger<LivenessRunner> logger)
            : base(context, notifier, logger)
        {
            _status = httpStatus;
            _content = content;
        }

        protected internal override Task<HttpResponseMessage> PerformRequest(string uri)
        {
            var message  =  new HttpResponseMessage(_status)
            {
                Content = new StringContent(_content,Encoding.UTF8,"application/json")
            };

            return Task.FromResult(message);
        }
    }
}
