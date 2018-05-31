using BeatPulse.Core;
using BeatPulse.MySql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMySql(this BeatPulseContext context, string connectionString, string defaultPath = "mysql")
        {
            return context.AddLiveness(nameof(MySqlLiveness), opt =>
            {
                opt.UseLiveness(new MySqlLiveness(connectionString));
                opt.UsePath(defaultPath);
            });
        }
    }
}
