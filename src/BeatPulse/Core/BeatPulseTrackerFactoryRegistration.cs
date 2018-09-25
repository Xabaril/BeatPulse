using System;

namespace BeatPulse.Core
{
    public class BeatPulseTrackerFactoryRegistration
        : IBeatPulseTrackerRegistration
    {
        private readonly Func<IServiceProvider, IBeatPulseTracker> _creator;

        public BeatPulseTrackerFactoryRegistration(Func<IServiceProvider, IBeatPulseTracker> creator)
        {
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        public IBeatPulseTracker GetOrCreateTracker(IServiceProvider serviceProvider) => _creator.Invoke(serviceProvider);
    }
}
