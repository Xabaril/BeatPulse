using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace BeatPulse.Redis
{
    public class RedisLiveness  : IHealthCheck
    {
        private readonly string _redisConnectionString;

        public RedisLiveness(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
