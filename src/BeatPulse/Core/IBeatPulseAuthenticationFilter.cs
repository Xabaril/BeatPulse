using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    interface IBeatPulseAuthenticationFilter
    {
        Task<bool> Valid(string apiKey);
    }
}
