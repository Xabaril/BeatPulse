using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString, string name = nameof(AzureBlobStorageLiveness), string defaultPath = "azureblobstorage")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureBlobStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString, string name = nameof(AzureTableStorageLiveness), string defaultPath = "azuretablestorage")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureTableStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString,string name = nameof(AzureQueueStorageLiveness), string defaultPath = "azurequeuestorage")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureQueueStorageLiveness(connectionString));
                setup.UsePath(defaultPath);
            });
        }
    }
}
