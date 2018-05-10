using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading;

namespace BeatPulse.AzureStorage
{
    public class AzureBlobStorageLiveness : IBeatPulseLiveness
    {
        CloudStorageAccount storageAccount;
        public string Name => nameof(AzureBlobStorageLiveness);

        public string Path { get; }

        public AzureBlobStorageLiveness(string connectionString, string defaultPath)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
            Path = defaultPath ?? throw new ArgumentNullException(nameof(defaultPath));
        }

        public async System.Threading.Tasks.Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync(new BlobRequestOptions(), null, cancellationToken);
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
