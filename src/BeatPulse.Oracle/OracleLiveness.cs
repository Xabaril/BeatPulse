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

        public OracleLiveness(string connectionString, string sql, ILogger<OracleLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
            _logger = logger;
        }
        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
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

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(OracleLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors);
            }
        }
    }
}
