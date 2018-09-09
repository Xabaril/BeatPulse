using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.AzureStorage
{
    public class AzureBlobStorageLiveness : IBeatPulseLiveness
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly ILogger<AzureBlobStorageLiveness> _logger;

        public AzureBlobStorageLiveness(string connectionString,ILogger<AzureBlobStorageLiveness> logger = null)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(AzureBlobStorageLiveness)} is checking the Azure Blob.");

                var blobClient = _storageAccount.CreateCloudBlobClient();

                var serviceProperties = await blobClient.GetServicePropertiesAsync(
                    new BlobRequestOptions(),
                    operationContext: null,
                    cancellationToken: cancellationToken);

                _logger?.LogInformation($"The {nameof(AzureBlobStorageLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(AzureBlobStorageLiveness)} check fail for {_storageAccount.BlobStorageUri} with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
