using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseService
    {
        Task<bool> EvaluateSegment(string segment);
    }
}
