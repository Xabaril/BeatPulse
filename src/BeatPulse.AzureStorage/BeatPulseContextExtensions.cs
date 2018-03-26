using BeatPulse.AzureStorage;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureBlobStorage(this BeatPulseContext context, string connectionString)
        {
            context.Add(new AzureBlobStorageLiveness(connectionString));
            return context;
        }

        public static BeatPulseContext AddAzureTableStorage(this BeatPulseContext context, string connectionString)
        {
            context.Add(new AzureTableStorageLiveness(connectionString));
            return context;
        }

        public static BeatPulseContext AddAzureQueueStorage(this BeatPulseContext context, string connectionString)
        {
            context.Add(new AzureQueueStorageLiveness(connectionString));
            return context;
        }
    }
}
