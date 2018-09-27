using BeatPulse.Core;
using BeatPulse.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddSqlServer(this BeatPulseContext context, string connectionString, string healthQuery = "SELECT 1;",string name = nameof(SqlServerLiveness), string defaultPath = "sqlserver")
        {
            if (healthQuery == null)
            {
                throw new System.ArgumentNullException(nameof(healthQuery));
            }

            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new SqlServerLiveness(connectionString, healthQuery, sp.GetService<ILogger<SqlServerLiveness>>()));
            });
        }
    }
}
