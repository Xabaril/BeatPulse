using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BeatPulse.Redis
{
    public class RedisHealthCheck
        : IBeatPulseHealthCheck
    {
        private string _redisConnectionString;

        public string HealthCheckName => nameof(RedisHealthCheck);

        public string HealthCheckDefaultPath => "redis";

        public RedisHealthCheck(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, HealthCheckName)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
            
        }
    }
}
