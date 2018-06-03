using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azureblobstorage")
        {
            return context.AddLiveness(nameof(AzureBlobStorageLiveness), setup =>
            {
                setup.UseLiveness(new AzureBlobStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azuretablestorage")
        {
            return context.AddLiveness(nameof(AzureTableStorageLiveness), setup =>
            {
                setup.UseLiveness(new AzureTableStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azurequeuestorage")
        {
            return context.AddLiveness(nameof(AzureQueueStorageLiveness), setup =>
            {
                setup.UseLiveness(new AzureQueueStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }
    }
}
