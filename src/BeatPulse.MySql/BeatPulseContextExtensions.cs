using BeatPulse.MySql;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddMySql(this IHealthChecksBuilder builder, string connectionString, string name = nameof(MySqlLiveness), string defaultPath = "mysql")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new MySqlLiveness(connectionString));
        }
    }
}
