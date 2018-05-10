using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azureblobstorage")
        {
            context.Add(new AzureBlobStorageLiveness(connectionString, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azuretablestorage")
        {
            context.Add(new AzureTableStorageLiveness(connectionString, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azurequeuestorage")
        {
            context.Add(new AzureQueueStorageLiveness(connectionString, defaultPath));
            return context;
        }
    }
}
