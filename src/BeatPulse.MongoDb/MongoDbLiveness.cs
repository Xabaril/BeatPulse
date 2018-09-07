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

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(MongoDbLiveness)} is checking the MongoDb database.");

                await new MongoClient(_connectionString)
                    .ListDatabasesAsync(cancellationToken);

                _logger?.LogInformation($"The {nameof(MongoDbLiveness)} check success for {_connectionString}");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(MongoDbLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}