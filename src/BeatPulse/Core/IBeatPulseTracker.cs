using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseTracker
    {
        Task Track(LivenessResult response);
    }
}