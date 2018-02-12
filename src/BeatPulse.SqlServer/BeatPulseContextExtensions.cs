using BeatPulse.Core;

namespace BeatPulse.SqlServer
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
