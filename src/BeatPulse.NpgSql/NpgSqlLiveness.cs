using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.NpgSql
{
    public class NpgSqlLiveness
        : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly ILogger<NpgSqlLiveness> _logger;

        public NpgSqlLiveness(string npgsqlConnectionString, string sql, ILogger<NpgSqlLiveness> logger = null)
        {
            _connectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    _logger?.LogDebug($"{nameof(NpgSqlLiveness)} is checking the PostgreSql using the query {_sql}.");

                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;

                        await command.ExecuteScalarAsync();

                        _logger?.LogDebug($"The {nameof(NpgSqlLiveness)} check success for {_connectionString}");
                    }

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogDebug($"The {nameof(NpgSqlLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}