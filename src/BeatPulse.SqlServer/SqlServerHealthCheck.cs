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
        private readonly SqlServerHcOptions _options;

        public string HealthCheckName { get; }

        public string HealthCheckDefaultPath => "sqlserver";

        public IHealthCheckOptions Options => _options;

        public SqlServerHealthCheck(string name, SqlServerHcOptions options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                throw new ArgumentException("No connection string provided. Please Use options.UseConnectionString().", "options");
            }
            HealthCheckName = name ?? nameof(SqlServerHealthCheck);
            _options = options;
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context)
        {
            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    return ("", true);
                }
                catch (Exception ex)
                {
                    return ($"Exception {ex.GetType().Name} with message ('{ex.Message}')", false);
                }
            }
        }
    }
}
