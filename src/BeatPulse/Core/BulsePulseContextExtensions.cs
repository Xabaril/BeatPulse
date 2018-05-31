using System;

namespace BeatPulse.Core
{
    public static class BulsePulseContextExtensions
    {
        public static BeatPulseContext AddLiveness(this BeatPulseContext ctx, string name, Action<BeatPulseLivenessRegistrationOptions> optionsAction)
        {
            var options = new BeatPulseLivenessRegistrationOptions(name);
            optionsAction(options);
            ctx.AddLiveness(BeatPulseLivenessRegistrationCreator.CreateUsingOptions(options));
            return ctx;
        }


        public static BeatPulseContext AddTracker(this BeatPulseContext ctx, IBeatPulseTracker tracker)
            => ctx.AddTracker(new BeatPulseTrackerInstanceRegistration(tracker));

        public static BeatPulseContext AddTracker(this BeatPulseContext ctx, string name, Func<IServiceProvider, IBeatPulseTracker> creator)
            => ctx.AddTracker(new BeatPulseTrackerFactoryRegistration(name, creator));
    }
}
