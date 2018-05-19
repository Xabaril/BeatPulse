using BeatPulse.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace BeatPulse.Tracker.ApplicationInsights
{
    public class ApplicationInsightsTracker
        : IBeatPulseTracker
    {
        public string Name => nameof(ApplicationInsightsTracker);

        private TelemetryClient _telemetryClient;

        public ApplicationInsightsTracker(string instrumentationKey = null)
        {
            var configuration = instrumentationKey != null
                ? new TelemetryConfiguration(instrumentationKey)
                : TelemetryConfiguration.Active;

            _telemetryClient = new TelemetryClient(configuration);
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
                {"ResponseTime",livenessResult.MilliSeconds }
            };


            _telemetryClient.TrackEvent($"BeatPulse", properties, metrics);

            return Task.CompletedTask;
        }
    }
}