using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.MongoDb
{
    public class MongoDbLiveness
        : IBeatPulseLiveness
    {
        private readonly string _mongoDbConnectionString;

        public MongoDbLiveness(string mongoDbConnectionString)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                await new MongoClient(_mongoDbConnectionString)
                    .ListDatabasesAsync(cancellationToken);

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
