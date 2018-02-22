using BeatPulse.Core;
using BeatPulse.SqlServer;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, Action<SqlServerHcOptions> optionsAction) => AddSqlServer(context, null, optionsAction);

        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string name, Action<SqlServerHcOptions> optionsAction)
        {
            var options = new SqlServerHcOptions();
            optionsAction.Invoke(options);
            context.Add(new SqlServerHealthCheck(name, options));
            return context;
        }
    }
}
