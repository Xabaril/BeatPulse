using BeatPulse.UI.Core.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    interface ILivenessRunner
    {
        Task Run(CancellationToken cancellationToken);

        Task<LivenessExecution> GetLatestRun(string livenessName,CancellationToken cancellationToken);

        Task<List<LivenessConfiguration>> GetConfiguredLiveness(CancellationToken cancellationToken);
        Task<int> AddConfiguredLiveness(LivenessConfiguration livenessConfiguration);
    }
}
