using BeatPulse.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace BeatPulse.ApplicationInsightsTracker
{
    public class AITracker
        : IBeatPulseTracker
    {
        const string EVENT_NAME = "BeatPulse";
        const string METRIC_NAME = "BeatPulse:ResponseTime";

        public string Name => nameof(AITracker);

        private TelemetryClient _telemetryClient;

        public AITracker()
        {
            _telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);
        }

        public Task Track(LivenessResult livenessResult)
        {
            var properties = new Dictionary<string, string>()
            {
                {nameof(livenessResult.IsHealthy),livenessResult.IsHealthy.ToString(CultureInfo.InvariantCulture)},
                {nameof(livenessResult.Run),livenessResult.Run.ToString(CultureInfo.InvariantCulture)},
                {nameof(livenessResult.Path),livenessResult.Path},
                {nameof(livenessResult.Name),livenessResult.Name}
            };

            var metrics = new Dictionary<string, double>()
            {
                {METRIC_NAME,livenessResult.MilliSeconds }
            };


            _telemetryClient.TrackEvent(EVENT_NAME, properties, metrics);

            return Task.CompletedTask;
        }
    }
}