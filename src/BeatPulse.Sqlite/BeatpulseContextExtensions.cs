using BeatPulse.Core;

namespace BeatPulse.Sqlite
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
                setup.UseLiveness(new SqliteLiveness(sqliteConnectionString, healthQuery));
                setup.UsePath(defaultPath);
            });
        }
    }
}
