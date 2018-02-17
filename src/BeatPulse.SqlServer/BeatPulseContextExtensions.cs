using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string sqlServerConnectionString)
        {
            context.Add(new SqlServerHealthCheck(sqlServerConnectionString));

            return context;
        }
    }
}
