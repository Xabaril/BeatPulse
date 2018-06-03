using BeatPulse.Core;
using BeatPulse.Redis;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRedis(this BeatPulseContext context, string redisConnectionString, string defaultPath = "redis")
        {
            return context.AddLiveness(nameof(RedisLiveness), setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new RedisLiveness(redisConnectionString));
            });
        }
    }
}
