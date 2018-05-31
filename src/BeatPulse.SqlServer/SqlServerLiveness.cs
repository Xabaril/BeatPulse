using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.SqlServer
{
    public class SqlServerLiveness
        : IBeatPulseLiveness
    {
        
        private readonly string _connectionString;

        public SqlServerLiveness(string sqlserverconnectionstring)
        {
            _connectionString = sqlserverconnectionstring ?? throw new ArgumentNullException(nameof(sqlserverconnectionstring));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext, CancellationToken cancellationToken = default)
        {

            using (var connection = new SqlConnection(_connectionString))
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
