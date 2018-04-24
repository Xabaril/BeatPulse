using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    class NoAuthenticationFilter : IBeatPulseAuthenticationFilter
    {
        public Task<bool> Valid(string apiKey)
        {
            return Task.FromResult(true);
        }
    }
}
