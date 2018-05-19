using BeatPulse.Core;
using BeatPulse.NpgSql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddNpgSql(this BeatPulseContext context, string npgsqlConnectionString, string defaultPath = "npgsql")
        {
            context.AddLiveness(new NpgSqlLiveness(npgsqlConnectionString,defaultPath));

            return context;
        }
    }
}
