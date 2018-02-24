using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BeatPulse.SqlServer
{
    public class SqlServerHealthCheck
        : IBeatPulseHealthCheck
    {
        public string HealthCheckName => nameof(SqlServerHealthCheck);

        public string HealthCheckDefaultPath => "sqlserver";

        private readonly string _connectionString;

        public SqlServerHealthCheck(string sqlserverconnectionstring)
        {
            _connectionString = sqlserverconnectionstring ?? throw new ArgumentNullException(nameof(sqlserverconnectionstring));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
                catch (Exception ex)
                {
                    var message = isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, HealthCheckName)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);
                }
            }
        }
    }
}
