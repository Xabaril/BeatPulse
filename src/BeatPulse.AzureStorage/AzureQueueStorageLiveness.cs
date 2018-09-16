using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BeatPulse.AzureStorage
{
    public class AzureQueueStorageLiveness : IHealthCheck
    {
        private readonly CloudStorageAccount _storageAccount;

        public AzureQueueStorageLiveness(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = _storageAccount.CreateCloudQueueClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync(
                    new QueueRequestOptions(),
                    operationContext: null,
                    cancellationToken: cancellationToken);

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
