using BeatPulse.Core;
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

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
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
                    var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);
                }
            }
        }
    }
}