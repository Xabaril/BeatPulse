﻿using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.SqlServer
{
    public class SqlServerLiveness
        : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly ILogger<SqlServerLiveness> _logger;

        public SqlServerLiveness(string sqlserverconnectionstring, string sql, ILogger<SqlServerLiveness> logger = null)
        {
            _connectionString = sqlserverconnectionstring ?? throw new ArgumentNullException(nameof(sqlserverconnectionstring));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    _logger?.LogInformation($"{nameof(SqlServerLiveness)} is checking the SqlServer using the query {_sql}.");

                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;

                        await command.ExecuteScalarAsync();
                    }

                    _logger?.LogInformation($"The {nameof(SqlServerLiveness)} check success for {_connectionString}");

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SqlServerLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
