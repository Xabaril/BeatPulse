using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseTracker
    {
        string Name { get; }

        Task Track(LivenessResult response);
    }
}