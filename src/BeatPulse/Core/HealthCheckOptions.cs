using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BeatPulse.Core
{
    public class HealthCheckOptions : IHealthCheckOptions
    {

        public bool IncludeInOutput { get; set; }

        public HealthCheckOptions()
        {
            IncludeInOutput = true;
        }


    }
}
