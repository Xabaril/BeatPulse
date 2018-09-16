using BeatPulse.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddSqlServer(this IHealthChecksBuilder builder, string connectionString,string name = nameof(SqlServerLiveness), string defaultPath = "sqlserver")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new SqlServerLiveness(connectionString));
        }
    }
}
