using System;

namespace BeatPulse.Core
{

    public class BeatPulseTrackerInstanceRegistration
        : IBeatPulseTrackerRegistration
    {
        private readonly IBeatPulseTracker _instance;

        public string Name => _instance.Name;

        public BeatPulseTrackerInstanceRegistration(IBeatPulseTracker tracker)
        {
            _instance = tracker ?? throw new ArgumentNullException(nameof(tracker));
        }

        public IBeatPulseTracker GetOrCreateTracker(IServiceProvider serviceProvider) => _instance;
    }
}
