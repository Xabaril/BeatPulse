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

        public NpgSqlHealthCheck(string npgsqlConnectionString)
        {
            _npgsqlConnectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
        }

        public async Task<bool> IsHealthy(HttpContext context)
        {
            using (var connection = new NpgsqlConnection(_npgsqlConnectionString))
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
