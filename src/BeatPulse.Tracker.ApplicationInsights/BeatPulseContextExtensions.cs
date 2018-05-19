using BeatPulse.Core;
using BeatPulse.Tracker.ApplicationInsights;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddApplicationInsightsTracker(this BeatPulseContext context, string instrumentationKey = null)
        {
            context.AddTracker(new ApplicationInsightsTracker(instrumentationKey));

            return context;
        }
    }
}
