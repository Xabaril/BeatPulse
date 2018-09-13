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

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    _logger?.LogInformation($"{nameof(SqliteLiveness)} is checking the Sqlite using the query {_sql}.");

                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;
                        await command.ExecuteScalarAsync();    
                    }

                    _logger?.LogInformation($"The {nameof(SqliteLiveness)} check success for {_connectionString}");

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SqliteLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors);
            }
        }
    }
}
