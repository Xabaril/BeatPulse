using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace BeatPulse.NpgSql
{
    public class NpgSqlLiveness : IHealthCheck
    {
        private readonly string _npgsqlConnectionString;

        public NpgSqlLiveness(string npgsqlConnectionString)
        {
            _npgsqlConnectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new NpgsqlConnection(_npgsqlConnectionString))
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