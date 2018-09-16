using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace BeatPulse.MongoDb
{
    public class MongoDbLiveness : IHealthCheck
    {
        private readonly string _mongoDbConnectionString;

        public MongoDbLiveness(string mongoDbConnectionString)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await new MongoClient(_mongoDbConnectionString).ListDatabasesAsync(cancellationToken);

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
