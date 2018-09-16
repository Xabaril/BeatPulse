using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Sqlite
{
    public class SqliteLiveness : IHealthCheck
    {
        private string _connectionString;
        private string _healthQuery;

        public SqliteLiveness(string connectionString, string healthQuery)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _healthQuery = healthQuery ?? throw new ArgumentException(nameof(healthQuery));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            SqliteConnection connection = null;

            try
            {
                using (connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var selectCmd = connection.CreateCommand();
                    selectCmd.CommandText = _healthQuery;
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        await reader.ReadAsync();
                    }

                    return HealthCheckResult.Passed();
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
