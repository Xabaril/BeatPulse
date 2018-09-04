using BeatPulse.Core;
using BeatPulse.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string name = nameof(SqliteLiveness), string defaultPath = "sqlite")
        {
            return AddSqlite(context, sqliteConnectionString, "select name from sqlite_master where type='table'", name, defaultPath);
        }

        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string healthQuery, string name = nameof(SqliteLiveness), string defaultPath = "sqlite")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new SqliteLiveness(sqliteConnectionString, healthQuery, sp.GetService<ILogger<SqliteLiveness>>()));
            });
        }
    }
}
