using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var data = livenessResult.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.PropertyType == typeof(string) || m.PropertyType == typeof(bool))
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(livenessResult, null).ToString());

            var metrics = new Dictionary<string, double>();
            metrics.Add("Milliseconds", livenessResult.MilliSeconds);
            _telemetryClient.TrackEvent($"BeatPulse", data, metrics);

        }

    }
}
