using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace BeatPulse.NpgSql
{
    public class NpgSqlHealthCheck
        : IBeatPulseHealthCheck
    {
        private readonly string _npgsqlConnectionString;

        public string HealthCheckName => nameof(NpgSqlHealthCheck);

        public string HealthCheckDefaultPath => "npgsql";

        public IHealthCheckOptions Options { get; }

        public NpgSqlHealthCheck(string npgsqlConnectionString)
        {
            _npgsqlConnectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
            Options = new HealthCheckOptions();
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context)
        {
            using (var connection = new NpgsqlConnection(_npgsqlConnectionString))
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
