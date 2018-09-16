using BeatPulse.AzureStorage;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddAzureBlobStorage(this IHealthChecksBuilder builder, string connectionString, string name = nameof(AzureBlobStorageLiveness), string defaultPath = "azureblobstorage")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureBlobStorageLiveness(connectionString));
        }

        public static IHealthChecksBuilder AddAzureTableStorage(this IHealthChecksBuilder builder, string connectionString, string name = nameof(AzureTableStorageLiveness), string defaultPath = "azuretablestorage")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureTableStorageLiveness(connectionString));
        }

        public static IHealthChecksBuilder AddAzureQueueStorage(this IHealthChecksBuilder builder, string connectionString,string name = nameof(AzureQueueStorageLiveness), string defaultPath = "azurequeuestorage")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureQueueStorageLiveness(connectionString));
        }
    }
}
