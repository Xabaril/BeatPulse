using BeatPulse.Core;
using BeatPulse.NpgSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddNpgSql(this BeatPulseContext context, string npgsqlConnectionString, string healthQuery = "SELECT 1;", string name = nameof(NpgSqlLiveness), string defaultPath = "npgsql")
        {
            return context.AddLiveness(name, setup =>
           {
               setup.UsePath(defaultPath);
               setup.UseFactory(sp=>new NpgSqlLiveness(npgsqlConnectionString,healthQuery,sp.GetService<ILogger<NpgSqlLiveness>>()));
           });
        }
    }
}
