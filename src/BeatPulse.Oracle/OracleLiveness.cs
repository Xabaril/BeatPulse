using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Oracle.ManagedDataAccess.Client;

namespace BeatPulse.Oracle
{
    public class OracleLiveness : IHealthCheck
    {
        private readonly string _connectionString;

        public OracleLiveness(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM V$VERSION";
                    await command.ExecuteScalarAsync();

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
