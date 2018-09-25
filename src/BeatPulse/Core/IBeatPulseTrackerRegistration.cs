using System;

namespace BeatPulse.Core
{
    public interface IBeatPulseTrackerRegistration
    {
        IBeatPulseTracker GetOrCreateTracker(IServiceProvider serviceProvider);
    }
}
