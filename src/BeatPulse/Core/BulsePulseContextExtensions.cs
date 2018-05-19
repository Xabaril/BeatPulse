using System;

namespace BeatPulse.Core
{
    public static class BulsePulseContextExtensions
    {
        public static BeatPulseContext AddLiveness(this BeatPulseContext ctx, IBeatPulseLiveness liveness)
            => ctx.AddLiveness(new BeatPulseLivenessInstanceRegistration(liveness));

        public static BeatPulseContext AddLiveness(this BeatPulseContext ctx, string path, Func<IServiceProvider, IBeatPulseLiveness> creator)
            => ctx.AddLiveness(new BeatPulseLivenessFactoryRegistration(path, creator));

        public static BeatPulseContext AddTracker(this BeatPulseContext ctx, IBeatPulseTracker tracker)
            => ctx.AddTracker(new BeatPulseTrackerInstanceRegistration(tracker));

        public static BeatPulseContext AddTracker(this BeatPulseContext ctx, string name, Func<IServiceProvider, IBeatPulseTracker> creator)
            => ctx.AddTracker(new BeatPulseTrackerFactoryRegistration(name, creator));
    }
}
