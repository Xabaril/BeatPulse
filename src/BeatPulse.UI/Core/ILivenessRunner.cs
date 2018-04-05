using BeatPulse.UI.Core.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    internal interface ILivenessRunner
    {
        Task Run(CancellationToken cancellationToken);

        Task<List<LivenessExecutionHistory>> GetLatestRun(string livenessName,CancellationToken cancellationToken);

        Task<List<LivenessConfiguration>> GetLiveness(CancellationToken cancellationToken);
    }
}
