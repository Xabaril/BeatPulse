using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core.Builders
{
    class FakeLivenessRunner : LivenessRunner
    {
        private readonly HttpStatusCode _status;
        private readonly string _content;

        public FakeLivenessRunner(HttpStatusCode httpStatus,string content,
            LivenessDb context, 
            ILivenessFailureNotifier notifier,
            IOptions<BeatPulseSettings> settings,
            ILogger<LivenessRunner> logger)
            : base(context, notifier,settings, logger)
        {
            _status = httpStatus;
            _content = content;
        }

        protected internal override Task<HttpResponseMessage> PerformRequest(string uri, CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage(_status)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(message);
        }
    }
}
