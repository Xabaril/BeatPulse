using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azureblobstorage")
        {
            context.AddLiveness(new AzureBlobStorageLiveness(connectionString, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azuretablestorage")
        {
            context.AddLiveness(new AzureTableStorageLiveness(connectionString, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azurequeuestorage")
        {
            context.AddLiveness(new AzureQueueStorageLiveness(connectionString, defaultPath));
            return context;
        }
    }
}
