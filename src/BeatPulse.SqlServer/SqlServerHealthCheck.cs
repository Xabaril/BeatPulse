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

        public IHealthCheckOptions Options { get; }

        string _connectionString;

        public SqlServerHealthCheck(string sqlserverconnectionstring)
        {
            _connectionString = sqlserverconnectionstring ?? throw new ArgumentNullException(nameof(sqlserverconnectionstring));
            Options = new HealthCheckOptions();
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    return ("OK", true);
                }
                catch (Exception ex)
                {
                    return ($"Exception {ex.GetType().Name} with message ('{ex.Message}')", false);
                }
            }
        }
    }
}
