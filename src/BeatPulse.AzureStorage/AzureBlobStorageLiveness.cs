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

        public AzureBlobStorageLiveness(string connectionString)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async System.Threading.Tasks.Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync(new BlobRequestOptions(), null, cancellationToken);
                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var isDevelopment = livenessContext.IsDevelopment;
                var name = livenessContext.Name;
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
