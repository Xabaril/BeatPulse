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

        public MongoDbHealthCheck(string mongoDbConnectionString)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
        }

        public async Task<bool> IsHealthy(HttpContext context)
        {
            try
            {
                await new MongoClient(_mongoDbConnectionString)
                    .ListDatabasesAsync();

                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
