using System;
using BeatPulse.Core;
using BeatPulse.StatusPageTracker;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddStatusPageTracker(this IHealthChecksBuilder builder,Action<StatusPageComponent> setup)
        {
            var component = new StatusPageComponent();
            setup(component);

            builder.Services.AddSingleton<IBeatPulseTracker>(new StatusPageIOTracker(component));
            return builder;
        }
    }
}
