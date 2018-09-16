using BeatPulse.NpgSql;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddNpgSql(this IHealthChecksBuilder builder, string npgsqlConnectionString, string name = nameof(NpgSqlLiveness), string defaultPath = "npgsql")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new NpgSqlLiveness(npgsqlConnectionString));
        }
    }
}
