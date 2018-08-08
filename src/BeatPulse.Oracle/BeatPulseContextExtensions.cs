using BeatPulse.Core;
using BeatPulse.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddOracle(this BeatPulseContext context, string connectionString, string name = nameof(OracleLiveness), string defaultPath = "oracle")
        {
            return context.AddLiveness(name, setup => {
                setup.UsePath(defaultPath);                
                setup.UseLiveness(new OracleLiveness(connectionString));
            });
        }
    }
}
