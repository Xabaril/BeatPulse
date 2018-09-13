using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.MySql
{
    public class MySqlLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly ILogger<MySqlLiveness> _logger;

        public MySqlLiveness(string connectionString, ILogger<MySqlLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(MySqlLiveness)} is checking the MySql.");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    if (!await connection.PingAsync(cancellationToken))
                    {
                        _logger?.LogWarning($"The {nameof(MySqlLiveness)} check fail for {_connectionString}.");

                        return LivenessResult.UnHealthy($"The {nameof(MySqlLiveness)} check fail.");
                    }

                    _logger?.LogInformation($"The {nameof(MySqlLiveness)} check success for {_connectionString}");

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(MySqlLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors);
            }
        }
    }
}
