using BeatPulse.Core;
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

        public AzureBlobStorageLiveness(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = _storageAccount.CreateCloudBlobClient();

                var serviceProperties = await blobClient.GetServicePropertiesAsync(
                    new BlobRequestOptions(),
                    operationContext: null,
                    cancellationToken: cancellationToken);

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
