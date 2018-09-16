using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySql.Data.MySqlClient;

namespace BeatPulse.MySql
{
    public class MySqlLiveness : IHealthCheck
    {
        private readonly string _connectionString;

        public MySqlLiveness(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    if (await connection.PingAsync(cancellationToken))
                    {
                        return HealthCheckResult.Passed();
                    }
                    else
                    {
                        return HealthCheckResult.Failed(string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Registration.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
