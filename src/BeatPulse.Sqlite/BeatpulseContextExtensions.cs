using BeatPulse.Core;
using System;

namespace BeatPulse.Sqlite
{
    public static class BeatPulseContextExtensions
    {
        /// <summary>
        /// Register Sqlite liveness on Beatpulse
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqliteConnectionString">Sqlite database connection string</param>
        /// <returns></returns>
        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString)
        {
            return AddSqlite(context, sqliteConnectionString, "select name from sqlite_master where type='table'");
        }

        /// <summary>
        /// Register Sqlite liveness on Beatpulse
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqliteConnectionString">Sqlite database connection string</param>
        /// <param name="healthQuery">Health query that will be executed by the database reader</param>
        /// <returns></returns>
        public static BeatPulseContext AddSqlite(this BeatPulseContext context, string sqliteConnectionString, string healthQuery)
        {
            context.Add(new SqliteLiveness(sqliteConnectionString, healthQuery));
            return context;
        }
    }
}
