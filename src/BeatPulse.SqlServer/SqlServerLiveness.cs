using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.SqlServer
{
    public class SqlServerLiveness : IHealthCheck
    {
        private readonly string _connectionString;

        public SqlServerLiveness(string sqlserverconnectionstring)
        {
            _connectionString = sqlserverconnectionstring ?? throw new ArgumentNullException(nameof(sqlserverconnectionstring));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT 1;";
                        await command.ExecuteScalarAsync();
                    }

                    return HealthCheckResult.Passed();
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Failed(exception: ex);
                }
            }
        }
    }
}
