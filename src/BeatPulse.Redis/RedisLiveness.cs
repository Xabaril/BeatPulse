using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Redis
{
    public class RedisLiveness
        : IBeatPulseLiveness
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections
            = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        private readonly string _redisConnectionString;
        private readonly ILogger<RedisLiveness> _logger;

        public RedisLiveness(string redisConnectionString, ILogger<RedisLiveness> logger = null)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
            _logger = logger;
        }

        public Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(RedisLiveness)} is checking the Redis status.");

                ConnectionMultiplexer connection;

                if (!_connections.TryGetValue(_redisConnectionString, out connection))
                {
                    connection = ConnectionMultiplexer.Connect(_redisConnectionString);

                    if (!_connections.TryAdd(_redisConnectionString, connection))
                    {
                        return Task.FromResult(
                            LivenessResult.UnHealthy("Redis connection can't be added into the dictionary."));
                    }
                }

                connection.GetDatabase()
                    .Ping();

                _logger?.LogInformation($"The {nameof(RedisLiveness)} check success.");

                return Task.FromResult(
                    LivenessResult.Healthy());
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(RedisLiveness)} check fail with the exception {ex.ToString()}.");

                return Task.FromResult(
                    LivenessResult.UnHealthy(ex));
            }
        }
    }
}
