using System;
using BeatPulse.Core;

namespace BeatPulse.Tracker.ApplicationInsights
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddApplicationInsightsTracker(this BeatPulseContext context,
            string instrumentationKey)
        {
            context.AddTracker(new ApplicationInsightsTracker(instrumentationKey));

            return context;

        }

        public static BeatPulseContext AddApplicationInsightsTracker(this BeatPulseContext context)
        {
            context.AddTracker(new ApplicationInsightsTracker());
            return context;
        }
    }
}
