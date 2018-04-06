using BeatPulse.Core;
using BeatPulse.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString, string defaultPath = "sqlserver")
        {
            var index = context
                .AllLivenesOfType<SqlServerLiveness>()
                .Where(l => l.DefaultPath.StartsWith(defaultPath))
                .Count();
            var path = index > 0 ? $"{defaultPath}-{index}" : defaultPath;
            context.Add(new SqlServerLiveness(connectionString, path));
            return context;
        }
    }
}
