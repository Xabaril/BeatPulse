using BeatPulse.Core;
using BeatPulse.MySql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMySql(this BeatPulseContext context, string connectionString, string name = nameof(MySqlLiveness), string defaultPath = "mysql")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new MySqlLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }
    }
}
