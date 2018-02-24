using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BeatPulse.MongoDb
{
    public class MongoDbHealthCheck
        : IBeatPulseHealthCheck
    {
        private readonly string _mongoDbConnectionString;

        public string HealthCheckName => nameof(MongoDbHealthCheck);

        public string HealthCheckDefaultPath => "mongodb";

        public MongoDbHealthCheck(string mongoDbConnectionString)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment)
        {
            try
            {
                await new MongoClient(_mongoDbConnectionString)
                    .ListDatabasesAsync();

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
