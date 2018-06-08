using BeatPulse.Core;
using BeatPulse.SqlServer;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString,string name = nameof(SqlServerLiveness), string defaultPath = "sqlserver")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new SqlServerLiveness(connectionString));
            });
        }
    }
}
