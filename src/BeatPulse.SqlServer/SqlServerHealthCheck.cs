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
        private readonly string _sqlServerConnectionString;

        public string HealthCheckName => nameof(SqlServerHealthCheck);

        public string HealthCheckDefaultPath => "sqlserver";

        public SqlServerHealthCheck(string sqlServerConnectionString)
        {
            _sqlServerConnectionString = sqlServerConnectionString ?? throw new ArgumentNullException(nameof(sqlServerConnectionString));
        }

        public async Task<bool> IsHealthy(HttpContext context)
        {
            using (var connection = new SqlConnection(_sqlServerConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
