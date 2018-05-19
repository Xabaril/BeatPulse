using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString, string defaultPath = "sqlserver")
        {
            context.AddLiveness(new SqlServerLiveness(connectionString, defaultPath));

            return context;
        }
    }
}
