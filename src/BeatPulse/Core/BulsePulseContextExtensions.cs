using BeatPulse.Core;
using System;

namespace BeatPulse
{
    /// <summary>
    /// <see cref="BeatPulseContext"/> extensions methods for BeatPulse
    /// </summary>
    public static class BulsePulseContextExtensions
    {
        /// <summary>
        /// Add a new liveness into the <see cref="BeatPulseContext"/>
        /// </summary>
        /// <param name="beatPulseContext">The <see cref="BeatPulseContext"/>.</param>
        /// <param name="name">The name of liveness to be added.</param>
        /// <param name="setup">Provided delegate to configure the liveness to be added.</param>
        /// <returns><see cref="BeatPulseContext"/></returns>
        public static BeatPulseContext AddLiveness(this BeatPulseContext beatPulseContext, string name, Action<BeatPulseLivenessRegistrationOptions> setup)
        {
            var options = new BeatPulseLivenessRegistrationOptions(name);
            setup(options);

            beatPulseContext.AddLiveness(options.CreateRegistration());

            return beatPulseContext;
        }

        /// <summary>
        /// Add a new tracker into the <see cref="BeatPulseContext"/>
        /// </summary>
        /// <param name="beatPulseContext">The <see cref="BeatPulseContext"/>.</param>
        /// <param name="tracker">The tracker to be added.</param>
        /// <returns><see cref="BeatPulseContext"/>.</returns>
        public static BeatPulseContext AddTracker(this BeatPulseContext beatPulseContext, IBeatPulseTracker tracker)
            => beatPulseContext.AddTracker(new BeatPulseTrackerInstanceRegistration(tracker));

        /// <summary>
        /// Add a new tracker into the <see cref="BeatPulseContext"/>
        /// </summary>
        /// <param name="beatPulseContext">The <see cref="BeatPulseContext"/>.</param>
        /// <param name="creator">Provided delegate to configure the tracker to be  added.</param>
        /// <returns><see cref="BeatPulseContext"/>.</returns>
        public static BeatPulseContext AddTracker(this BeatPulseContext beatPulseContext, Func<IServiceProvider, IBeatPulseTracker> creator)
            => beatPulseContext.AddTracker(new BeatPulseTrackerFactoryRegistration(creator));
    }
}
