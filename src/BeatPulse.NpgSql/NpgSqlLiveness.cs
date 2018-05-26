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
        public string Name => nameof(NpgSqlLiveness);

        public string Path { get; }

        private readonly string _npgsqlConnectionString;

        public NpgSqlLiveness(string npgsqlConnectionString, string defaultPath)
        {
            _npgsqlConnectionString = npgsqlConnectionString ?? throw new ArgumentNullException(nameof(npgsqlConnectionString));
            Path = defaultPath ?? throw new ArgumentNullException(nameof(Path));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
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
                    var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);

                }
            }
        }
    }
}
