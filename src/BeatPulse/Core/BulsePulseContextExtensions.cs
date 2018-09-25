using BeatPulse.Core;
using System;

namespace BeatPulse
{
    public static class BulsePulseContextExtensions
    {
        public static BeatPulseContext AddLiveness(this BeatPulseContext beatPulseContext, string name, Action<BeatPulseLivenessRegistrationOptions> setup)
        {
            var options = new BeatPulseLivenessRegistrationOptions(name);
            setup(options);

            beatPulseContext.AddLiveness(options.CreateRegistration());

            return beatPulseContext;
        }


        public static BeatPulseContext AddTracker(this BeatPulseContext beatPulseContext, IBeatPulseTracker tracker)
            => beatPulseContext.AddTracker(new BeatPulseTrackerInstanceRegistration(tracker));

        public static BeatPulseContext AddTracker(this BeatPulseContext beatPulseContext, Func<IServiceProvider, IBeatPulseTracker> creator)
            => beatPulseContext.AddTracker(new BeatPulseTrackerFactoryRegistration(creator));
    }
}
