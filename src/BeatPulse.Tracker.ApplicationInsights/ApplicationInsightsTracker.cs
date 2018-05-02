using System;
using System.Collections.Generic;
using System.Text;
using BeatPulse.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace BeatPulse.Tracker.ApplicationInsights
{
    public class ApplicationInsightsTracker : IBeatPulseTracker
    {
        public string Name => nameof(ApplicationInsightsTracker);

        private TelemetryClient _telemetryClient;

        public ApplicationInsightsTracker(string instrumentationKey)
        {
            _telemetryClient = new TelemetryClient(new TelemetryConfiguration(instrumentationKey));
        }

        public ApplicationInsightsTracker()
        {
            _telemetryClient = new TelemetryClient();
        }

        public void Track(LivenessResult livenessResult)
        {
            var data = new Dictionary<string, string>();
            data.Add("Name", livenessResult.Name);
            data.Add("Message", livenessResult.Message);
            data.Add("IsHealthy", livenessResult.IsHealthy.ToString());
            data.Add("Run", livenessResult.Run.ToString());

            var metrics = new Dictionary<string, double>();
            metrics.Add("Milliseconds", livenessResult.MilliSeconds);
            _telemetryClient.TrackEvent($"BeatPulse", data, metrics);
            
            ///TODO: Is worth or necessary?
            _telemetryClient.TrackMetric($"BeatPulse:{livenessResult.Name}", livenessResult.MilliSeconds, data);
        }

    }
}
