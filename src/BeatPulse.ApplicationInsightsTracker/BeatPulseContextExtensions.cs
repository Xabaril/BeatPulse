using BeatPulse.ApplicationInsightsTracker;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddApplicationInsightsTracker(this IHealthChecksBuilder builder)
        {
            builder.Services.AddSingleton<IBeatPulseTracker>(new AITracker());
            return builder;
        }

        public static IHealthChecksBuilder AddApplicationInsightsTracker(this IHealthChecksBuilder builder, string instrumentationKey)
        {
            builder.Services.AddSingleton<IBeatPulseTracker>(new AITracker(instrumentationKey));
            return builder;
        }
    }
}
