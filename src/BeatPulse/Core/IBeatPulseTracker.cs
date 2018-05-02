using System.Collections.Generic;

namespace BeatPulse.Core
{
    public interface IBeatPulseTracker
    {
        string Name { get; }
        void Track(IEnumerable<LivenessResult> responses);
    }
}