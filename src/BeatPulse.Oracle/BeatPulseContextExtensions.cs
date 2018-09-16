using BeatPulse.Oracle;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddOracle(this IHealthChecksBuilder builder, string connectionString, string name = nameof(OracleLiveness), string defaultPath = "oracle")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new OracleLiveness(connectionString));
        }
    }
}
