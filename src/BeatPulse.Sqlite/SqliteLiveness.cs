using BeatPulse.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Sqlite
{
    public class SqliteLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly ILogger<SqliteLiveness> _logger;

        public SqliteLiveness(string connectionString, string sql, ILogger<SqliteLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _sql = sql ?? throw new ArgumentException(nameof(sql));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    _logger?.LogDebug($"{nameof(SqliteLiveness)} is checking the Sqlite using the query {_sql}.");

                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;
                        await command.ExecuteScalarAsync();

                        _logger?.LogDebug($"The {nameof(SqliteLiveness)} check success for {_connectionString}");
                    }

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogDebug($"The {nameof(SqliteLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
