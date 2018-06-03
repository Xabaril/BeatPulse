using BeatPulse.Core;
using BeatPulse.NpgSql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddNpgSql(this BeatPulseContext context, string npgsqlConnectionString, string defaultPath = "npgsql")
        {
            return  context.AddLiveness(nameof(NpgSqlLiveness), setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new NpgSqlLiveness(npgsqlConnectionString));
            });
      }
    }
}
