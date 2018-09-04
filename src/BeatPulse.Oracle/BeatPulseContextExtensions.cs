using BeatPulse.Core;
using BeatPulse.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddOracle(this BeatPulseContext context, string connectionString, string healthQuery = "SELECT* FROM V$VERSION;", string name = nameof(OracleLiveness), string defaultPath = "oracle")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new OracleLiveness(connectionString, healthQuery, sp.GetService<ILogger<OracleLiveness>>()));
            });
        }
    }
}
