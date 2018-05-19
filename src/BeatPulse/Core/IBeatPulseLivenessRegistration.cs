using System;

namespace BeatPulse.Core
{
    public interface IBeatPulseLivenessRegistration
    {
        string Path { get; }

        IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider serviceProvider);
    }
}
