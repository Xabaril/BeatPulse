using BeatPulse.Core;
using BeatPulse.NpgSql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddNpgSql(this BeatPulseContext context, string npgsqlConnectionString)
        {
            context.Add(new NpgSqlHealthCheck(npgsqlConnectionString));

            return context;
        }
    }
}
