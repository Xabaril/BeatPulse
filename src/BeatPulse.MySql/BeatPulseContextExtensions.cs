using BeatPulse.Core;
using BeatPulse.MySql;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMySql(this BeatPulseContext context, string connectionString)
        {
            context.Add(new MySqlLiveness(connectionString));
            return context;
        }
    }
}
