using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Redis
{
    public class RedisLiveness
        : IBeatPulseLiveness
    {
        private readonly string _redisConnectionString;
        private readonly ILogger<RedisLiveness> _logger;

        public RedisLiveness(string redisConnectionString, ILogger<RedisLiveness> logger = null)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(RedisLiveness)} is checking the Redis status.");

                using (var connection = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString))
                {
                    _logger?.LogInformation($"The {nameof(RedisLiveness)} check success.");

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(RedisLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
