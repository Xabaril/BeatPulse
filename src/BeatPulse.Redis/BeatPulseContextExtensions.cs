using BeatPulse.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, string redisConnectionString, string name = nameof(RedisLiveness), string defaultPath = "redis")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new RedisLiveness(redisConnectionString));
        }
    }
}
