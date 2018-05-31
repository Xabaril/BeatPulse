using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azureblobstorage")
        {
            return context.AddLiveness(nameof(AzureBlobStorageLiveness), opt =>
            {
                opt.UseLiveness(new AzureBlobStorageLiveness(connectionString));
                opt.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azuretablestorage")
        {
            return context.AddLiveness(nameof(AzureTableStorageLiveness), opt =>
            {
                opt.UseLiveness(new AzureTableStorageLiveness(connectionString));
                opt.UsePath(defaultPath);
            });

        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString, string defaultPath = "azurequeuestorage")
        {
            return context.AddLiveness(nameof(AzureQueueStorageLiveness), opt =>
            {
                opt.UseLiveness(new AzureQueueStorageLiveness(connectionString));
                opt.UsePath(defaultPath);
            });

        }
    }
}
