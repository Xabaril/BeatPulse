using BeatPulse.Core;

namespace BeatPulse.Sqlite
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString)
        {
            return AddSqlite(context, sqliteConnectionString, "select name from sqlite_master where type='table'");
        }

        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string healthQuery)
        {
            context.Add(new SqliteLiveness(sqliteConnectionString, healthQuery));
            return context;
        }
    }
}
