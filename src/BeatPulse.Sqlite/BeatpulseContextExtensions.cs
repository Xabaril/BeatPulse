using BeatPulse.Core;

namespace BeatPulse.Sqlite
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string defaultPath = "sqlite")
        {
            return AddSqlite(context, sqliteConnectionString, "select name from sqlite_master where type='table'", defaultPath);
        }

        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string healthQuery, string defaultPath = "sqlite")
        {
            return context.AddLiveness(nameof(SqliteLiveness), opt =>
            {
                opt.UseLiveness(new SqliteLiveness(sqliteConnectionString, healthQuery));
                opt.UsePath(defaultPath);
            });

       }
    }
}
