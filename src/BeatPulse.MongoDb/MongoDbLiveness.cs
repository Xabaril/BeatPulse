using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.MongoDb
{
    public class MongoDbLiveness
        : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly ILogger<MongoDbLiveness> _logger;

        public MongoDbLiveness(string connectionString,ILogger<MongoDbLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(MongoDbLiveness)} is checking the MongoDb database.");

                var client = new MongoClient(_connectionString);
                
                var url = new MongoUrl(_connectionString);
                if (string.IsNullOrWhiteSpace(url.DatabaseName))
                    await client.ListDatabasesAsync(cancellationToken);
                else
                    await client.GetDatabase(url.DatabaseName).ListCollectionsAsync(cancellationToken: cancellationToken);

                _logger?.LogInformation($"The {nameof(MongoDbLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(MongoDbLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}