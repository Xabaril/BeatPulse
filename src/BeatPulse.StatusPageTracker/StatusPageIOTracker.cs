using BeatPulse.Core;
using System.Threading.Tasks;

namespace BeatPulse.StatusPageTracker
{
    class StatusPageIOTracker
        : IBeatPulseTracker
    {
        private readonly StatusPageClient _statusPageClient;

        public StatusPageIOTracker(StatusPageComponent statusPageComponent)
        {
            _statusPageClient = new StatusPageClient(statusPageComponent);
        }

        public async Task Track(LivenessResult response)
        {
            if (response.IsHealthy)
            {
                await _statusPageClient.SolveIncident();
            }
            else
            {
                await _statusPageClient.CreateIncident(response.Message);
            }
        }
    }
}
