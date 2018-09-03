using BeatPulse.Core;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.MySql
{
    public class MySqlLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;

        public MySqlLiveness(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    if (await connection.PingAsync(cancellationToken))
                    {
                        return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                    }
                    else
                    {
                        return (string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name), false);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
