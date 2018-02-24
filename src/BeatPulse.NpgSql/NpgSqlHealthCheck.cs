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

        public async Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment)
        {
            using (var connection = new NpgsqlConnection(_npgsqlConnectionString))
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
