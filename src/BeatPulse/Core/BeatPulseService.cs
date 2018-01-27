using System.Threading.Tasks;

namespace BeatPulse.Core
{
    class BeatPulseService
        : IBeatPulseService
    {
        public BeatPulseService()
        {
        }

        public Task<bool> EvaluateSegment(string segment)
        {
            return Task.FromResult<bool>(true);
        }
    }
}
