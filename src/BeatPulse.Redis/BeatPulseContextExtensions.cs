using BeatPulse.Core;
using BeatPulse.Redis;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRedis(this BeatPulseContext context, string redisConnectionString, string defaultPath = "redis")
        {
            context.Add(new RedisLiveness(redisConnectionString, defaultPath));

            return context;
        }
    }
}
