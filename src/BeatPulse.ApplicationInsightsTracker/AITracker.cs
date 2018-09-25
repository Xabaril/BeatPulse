using BeatPulse.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace BeatPulse.ApplicationInsightsTracker
{
    public class AITracker
        : IBeatPulseTracker
    {
        const string EVENT_NAME = "BeatPulse";
        const string RESPONSE_TIME_METRIC_NAME = "BeatPulse:ResponseTime";
        const string AVAILABILITY_METRIC_NAME = "BeatPulse:Availability";

        public string Name => nameof(AITracker);

        private readonly string _instrumentationKey;

        public AITracker(string instrumentationKey = null)
        {
            _instrumentationKey = instrumentationKey;
        }

        public Task Track(LivenessResult livenessResult)
        {
            var properties = new Dictionary<string, string>()
            {
                {nameof(livenessResult.IsHealthy),livenessResult.IsHealthy.ToString(CultureInfo.InvariantCulture)},
                {nameof(livenessResult.Run),livenessResult.Run.ToString(CultureInfo.InvariantCulture)},
                {nameof(livenessResult.Path),livenessResult.Path},
                {nameof(livenessResult.Name),livenessResult.Name},
                {nameof(Environment.MachineName),Environment.MachineName},
                {nameof(Assembly),Assembly.GetEntryAssembly().GetName().Name }
            };

            var metrics = new Dictionary<string, double>()
            {
                {RESPONSE_TIME_METRIC_NAME,livenessResult.Elapsed.TotalMilliseconds },
                {AVAILABILITY_METRIC_NAME,livenessResult.IsHealthy ? 1 : 0 }
            };

            var configuration = String.IsNullOrWhiteSpace(_instrumentationKey) ? TelemetryConfiguration.Active : new TelemetryConfiguration(_instrumentationKey);

            new TelemetryClient(configuration)
                .TrackEvent(EVENT_NAME, properties, metrics);

            return Task.CompletedTask;
        }
    }
}