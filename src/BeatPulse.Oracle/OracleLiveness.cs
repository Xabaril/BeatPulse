using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Oracle
{
    public class OracleLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly ILogger<OracleLiveness> _logger;

        public OracleLiveness(string connectionString,string sql,ILogger<OracleLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
            _logger = logger;
        }
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    _logger?.LogDebug($"{nameof(OracleLiveness)} is checking the Oracle using the query {_sql}.");

                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;
                        await command.ExecuteScalarAsync();
                    }

                    _logger?.LogDebug($"The {nameof(OracleLiveness)} check success for {_connectionString}");

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(OracleLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
