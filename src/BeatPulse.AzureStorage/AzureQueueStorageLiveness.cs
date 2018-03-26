using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading;

namespace BeatPulse.AzureStorage
{
    public class AzureQueueStorageLiveness : IBeatPulseLiveness
    {
        CloudStorageAccount storageAccount;
        public string Name => nameof(AzureQueueStorageLiveness);

        public string DefaultPath => "azurequeuestorage";

        public AzureQueueStorageLiveness(string connectionString)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async System.Threading.Tasks.Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudQueueClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync(new QueueRequestOptions(), null, cancellationToken);
                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
