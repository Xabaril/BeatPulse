using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeatPulse.AzureStorage
{
    public class AzureTableStorageLiveness : IHealthCheck
    {
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageLiveness(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = _storageAccount.CreateCloudTableClient();

                var serviceProperties = await blobClient.GetServicePropertiesAsync(
                    new TableRequestOptions(), 
                    operationContext:null,
                    cancellationToken:cancellationToken);

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
