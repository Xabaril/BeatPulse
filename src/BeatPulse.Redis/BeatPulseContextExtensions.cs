using BeatPulse.Core;
using BeatPulse.Redis;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRedis(this BeatPulseContext context, string redisConnectionString, string name = nameof(RedisLiveness), string defaultPath = "redis")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new RedisLiveness(redisConnectionString));
            });
        }
    }
}
