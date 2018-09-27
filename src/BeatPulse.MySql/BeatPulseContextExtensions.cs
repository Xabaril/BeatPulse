using BeatPulse.Core;
using BeatPulse.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMySql(this BeatPulseContext context, string connectionString, string name = nameof(MySqlLiveness), string defaultPath = "mysql")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new MySqlLiveness(connectionString, sp.GetService<ILogger<MySqlLiveness>>()));
            });
        }
    }
}
