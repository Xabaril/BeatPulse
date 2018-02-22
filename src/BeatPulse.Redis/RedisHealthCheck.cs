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

        public IHealthCheckOptions Options { get; }

        public RedisHealthCheck(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
            Options = new HealthCheckOptions();
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return ("OK", true);
            }
            catch (Exception ex)
            {
                return ($"Exception {ex.GetType().Name} with message ('{ex.Message}')", false);
            }
            
        }
    }
}
