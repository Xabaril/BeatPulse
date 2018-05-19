using System;

namespace BeatPulse.Core
{
    public interface IBeatPulseTrackerRegistration
    {
        string Name { get; }

        IBeatPulseTracker GetOrCreateTracker(IServiceProvider serviceProvider);
    }
}
