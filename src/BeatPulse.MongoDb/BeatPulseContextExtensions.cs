using BeatPulse.MongoDb;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddMongoDb(this IHealthChecksBuilder builder, string mongodbConnectionString, string name = nameof(MongoDbLiveness),string defaultPath = "mongodb")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new MongoDbLiveness(mongodbConnectionString));
        }
    }
}
