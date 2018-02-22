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
        private string _mongoDbConnectionString;

        public string HealthCheckName => nameof(MongoDbHealthCheck);

        public string HealthCheckDefaultPath => "mongodb";

        public IHealthCheckOptions Options { get; }

        public MongoDbHealthCheck(string mongoDbConnectionString)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
            Options = new HealthCheckOptions();
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context)
        {
            try
            {
                await new MongoClient(_mongoDbConnectionString)
                    .ListDatabasesAsync();

                return ("", true);
            }
            catch (Exception ex)
            {
                return ($"Exception {ex.GetType().Name} with message ('{ex.Message}')", false);
            }
            
        }
    }
}
