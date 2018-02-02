using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class BeatPulseChecks
    {
        public BeatPulseChecks Add(string name,IBeatPulseHealthCheck check)
        {
            return this;
        }
    }
}
