using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Threading;

namespace BeatPulse.MySql
{
    public class MySqlLiveness : IBeatPulseLiveness
    {
        private readonly string connectionString;

        public string Name => nameof(MySqlLiveness);

        public string DefaultPath => "mysql";

        public MySqlLiveness(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async System.Threading.Tasks.Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
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
