using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.SqlServer
{
    public class SqlServerHcOptions : HealthCheckOptions
    {
        public string ConnectionString { get; private set; }
        public void UseConnectionSring(string constr)
        {
            ConnectionString = constr;
        }

    }
}
