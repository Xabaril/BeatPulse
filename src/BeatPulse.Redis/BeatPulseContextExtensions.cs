using BeatPulse.Core;
using BeatPulse.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRedis(this BeatPulseContext context, string redisConnectionString, string name = nameof(RedisLiveness), string defaultPath = "redis")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp=>new RedisLiveness(redisConnectionString,sp.GetService<ILogger<RedisLiveness>>()));
            });
        }
    }
}
