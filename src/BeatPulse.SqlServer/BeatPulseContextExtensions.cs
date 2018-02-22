using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString)
        {
            context.Add(new SqlServerHealthCheck(connectionString));

            return context;
        }
    }
}
