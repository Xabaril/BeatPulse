using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString, string defaultPath = "sqlserver")
        {
            return context.AddLiveness(nameof(SqlServerLiveness), setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new SqlServerLiveness(connectionString));
            });
        }
    }
}
