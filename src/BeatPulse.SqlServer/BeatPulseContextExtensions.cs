using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString, string defaultPath = "sqlserver")
        {
            context.Add(new SqlServerLiveness(connectionString, defaultPath));

            return context;
        }
    }
}
