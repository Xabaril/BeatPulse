using System;

namespace BeatPulse.Core
{
    public class BeatPulseTrackerFactoryRegistration
        : IBeatPulseTrackerRegistration
    {
        private readonly Func<IServiceProvider, IBeatPulseTracker> _creator;

        public string Name { get; }

        public BeatPulseTrackerFactoryRegistration(string name, Func<IServiceProvider, IBeatPulseTracker> creator)
        {
            Name = name;
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        public IBeatPulseTracker GetOrCreateTracker(IServiceProvider serviceProvider) => _creator.Invoke(serviceProvider);
    }
}
