using BeatPulse.AzureServiceBus;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureEventHub(this BeatPulseContext context, string connectionString, string eventHubName, string defaultPath = "azureeventhub")
        {
            return context.AddLiveness(nameof(AzureEventHubLiveness), setup =>
            {
                setup.UseLiveness(new AzureEventHubLiveness(connectionString, eventHubName));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureServiceBusQueue(this BeatPulseContext context, string connectionString, string queueName, string defaultPath = "azureservicebusqueue")
        {
            return context.AddLiveness(nameof(AzureServiceBusQueueLiveness), setup =>
            {
                setup.UseLiveness(new AzureServiceBusQueueLiveness(connectionString, queueName));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureServiceBusTopic(this BeatPulseContext context, string connectionString, string topicName, string defaultPath = "azureservicebustopic")
        {
            return context.AddLiveness(nameof(AzureServiceBusTopicLiveness), setup =>
            {
                setup.UseLiveness(new AzureServiceBusTopicLiveness(connectionString, topicName));
                setup.UsePath(defaultPath);
            });
        }
    }
}
