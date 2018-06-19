using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public class OwinBeatPulseService : IBeatPulseService
    {
        private readonly BeatPulseContext _context;
        public OwinBeatPulseService(BeatPulseContext beatPulseContext)
        {
            _context = beatPulseContext;
        }

        public Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options)
        {
            return Task.FromResult(Enumerable.Empty<LivenessResult>());
        }
    }
}
