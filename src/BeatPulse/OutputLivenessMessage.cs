using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse
{
    class OutputLivenessMessage
    {
        private readonly List<HealthReportEntry> _messages = new List<HealthReportEntry>();

        public IEnumerable<HealthReportEntry> Checks => _messages;

        public DateTime StartedAtUtc { get; private set; }

        public DateTime EndAtUtc { get; private set; }

        public int Code { get; private set; }

        public string Reason { get; private set; }

        public OutputLivenessMessage()
        {
            StartedAtUtc = DateTime.UtcNow;
        }

        public void AddHealthCheckMessages(IEnumerable<HealthReportEntry> messages) => _messages.AddRange(messages);

        public void SetNotFound()
        {
            EndAtUtc = DateTime.UtcNow;
            Code = StatusCodes.Status503ServiceUnavailable;
            Reason = BeatPulseKeys.BEATPULSE_INVALIDPATH_REASON;
        }

        public void SetExecuted()
        {
            var isHealthy = Checks.All(x => x.Status >= HealthStatus.Degraded);

            EndAtUtc = DateTime.UtcNow;
            Code = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            Reason = isHealthy ? string.Empty : BeatPulseKeys.BEATPULSE_SERVICEUNAVAILABLE_REASON;
        }
    }
}
