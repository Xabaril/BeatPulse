using BeatPulse.ApplicationInsightsTracker;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddApplicationInsightsTracker(this BeatPulseContext context)
        {
            context.AddTracker(new AITracker());

            return context;
        }

        public static BeatPulseContext AddApplicationInsightsTracker(this BeatPulseContext context,string instrumentationKey)
        {
            context.AddTracker(new AITracker(instrumentationKey));

            return context;
        }
    }
}
