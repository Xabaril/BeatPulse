using System;
using System.Collections.Generic;
using BeatPulse.Core;
using BeatPulse.PrometheusTracker;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddPrometheusTracker(this IHealthChecksBuilder builder, Uri prometheusGatewayUri)
        {
            return AddPrometheusTracker(builder, prometheusGatewayUri, customLabels: null);
        }
        public static IHealthChecksBuilder AddPrometheusTracker(this IHealthChecksBuilder builder, Uri prometheusGatewayUri, IDictionary<string, string> customLabels)
        {
            builder.Services.AddSingleton<IBeatPulseTracker>(new PrometheusGatewayTracker(prometheusGatewayUri, customLabels));
            return builder;
        }
    }
}
