using System;

namespace BeatPulse.Core
{
    public interface IBeatPulseLivenessRegistration
    {
        string Name { get; }
        string Path { get; }

        IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider serviceProvider);
    }
}
