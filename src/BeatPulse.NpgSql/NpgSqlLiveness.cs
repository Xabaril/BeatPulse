using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.NpgSql
{
    public class NpgSqlLiveness
        : IBeatPulseLiveness
    {
        
        private readonly string _npgsqlConnectionString;

        public NpgSqlLiveness(string npgsqlConnectionString)
        {
            _npgsqlConnectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext, CancellationToken cancellationToken = default)
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

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
                catch (Exception ex)
                {
                    var isDevelopment = livenessContext.IsDevelopment;
                    var name = livenessContext.Name;
                    var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);

                }
            }
        }
    }
}
