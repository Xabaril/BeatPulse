using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Redis
{
    public class RedisLiveness
        : IBeatPulseLiveness
    {
        private string _redisConnectionString;

        public string Name => nameof(RedisLiveness);

        public string Path { get; }

        public RedisLiveness(string redisConnectionString, string defaultPath)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
            Path = defaultPath ?? throw new ArgumentNullException(nameof(defaultPath));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }

        }
    }
}
