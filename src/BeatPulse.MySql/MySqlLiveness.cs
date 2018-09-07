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

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
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

                        return (string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name), false);
                    }

                    _logger?.LogInformation($"The {nameof(MySqlLiveness)} check success for {_connectionString}");

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(MySqlLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
