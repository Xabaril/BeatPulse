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

        public async Task<bool> IsHealthy(HttpContext context)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
