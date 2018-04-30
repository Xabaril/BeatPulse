using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse
{
    class OutputLivenessMessage
    {
        private readonly List<LivenessResult> _messages = new List<LivenessResult>();

        public IEnumerable<LivenessResult> Checks => _messages;

        public DateTime StartedAtUtc { get; private set; }

        public DateTime EndAtUtc { get; private set; }

        public int Code { get; private set; }

        public string Reason { get; private set; }

        public OutputLivenessMessage()
        {
            StartedAtUtc = DateTime.UtcNow;
        }

        public void AddHealthCheckMessages(IEnumerable<LivenessResult> messages) => _messages.AddRange(messages);

        public void SetNotFound()
        {
            EndAtUtc = DateTime.UtcNow;
            Code = StatusCodes.Status503ServiceUnavailable;
            Reason = BeatPulseKeys.BEATPULSE_INVALIDPATH_REASON;
        }

        public void SetExecuted()
        {
            var isHealthy = Checks.All(x => x.IsHealthy);

            EndAtUtc = DateTime.UtcNow;
            Code = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            Reason = isHealthy ? string.Empty : BeatPulseKeys.BEATPULSE_SERVICEUNAVAILABLE_REASON;
        }
    }
}
