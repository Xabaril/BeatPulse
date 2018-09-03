using BeatPulse.Core;
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

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
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
                    var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);
                }
            }
        }
    }
}
