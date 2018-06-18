using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public interface IBeatPulseService
    {
        Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options);
    }
}
