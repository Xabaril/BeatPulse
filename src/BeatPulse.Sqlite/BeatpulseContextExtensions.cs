using BeatPulse.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddSqlite(this IHealthChecksBuilder builder, string sqliteConnectionString, string name = nameof(SqliteLiveness), string defaultPath = "sqlite")
        {
            return AddSqlite(builder, sqliteConnectionString, "select name from sqlite_master where type='table'", name, defaultPath);
        }

        public static IHealthChecksBuilder AddSqlite(this IHealthChecksBuilder builder, string sqliteConnectionString, string healthQuery, string name = nameof(SqliteLiveness), string defaultPath = "sqlite")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new SqliteLiveness(sqliteConnectionString, healthQuery));
        }
    }
}
