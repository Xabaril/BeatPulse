using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    class BeatPulseService
        : IBeatPulseService
    {
        private readonly IEnumerable<IBeatPulseHealthCheck> _checkers;

        public BeatPulseService(IEnumerable<IBeatPulseHealthCheck> checkers)
        {
            _checkers = checkers ?? throw new ArgumentNullException(nameof(checkers));
        }

       
        public Task<bool> IsHealthy(string segment)
        {
            return Task.FromResult(true);
        }
    }
}
