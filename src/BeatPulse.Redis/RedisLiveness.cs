using BeatPulse.Core;
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

        public RedisLiveness(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
