using BeatPulse.Core;
using BeatPulse.Redis;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRedis(this BeatPulseContext context, string redisConnectionString)
        {
            context.Add(new RedisHealthCheck(redisConnectionString));

            return context;
        }
    }
}
