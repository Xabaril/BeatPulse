using BeatPulse.Core;
using BeatPulse.PrometheusTracker;
using System;
using System.Collections.Generic;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddPrometheusTracker(this BeatPulseContext context, Uri prometheusGatewayUri)
        {
            context.AddPrometheusTracker(prometheusGatewayUri, null);
            return context;
        }
        public static BeatPulseContext AddPrometheusTracker(this BeatPulseContext context, Uri prometheusGatewayUri, IDictionary<string, string> customLabels)
        {
            context.AddTracker(new PrometheusGatewayTracker(prometheusGatewayUri, customLabels));
            return context;
        }
    }
}
