using BeatPulse.AzureServiceBus;
using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureEventHub(this BeatPulseContext context, string connectionString, string eventHubName, string defaultPath = "azureeventhub")
        {
            return context.AddLiveness(nameof(AzureEventHubLiveness), opt =>
            {
                opt.UseLiveness(new AzureEventHubLiveness(connectionString, eventHubName));
                opt.UsePath(defaultPath);
            });

        }

        public static BeatPulseContext AddAzureServiceBusQueue(this BeatPulseContext context, string connectionString, string queueName, string defaultPath = "azureservicebusqueue")
        {
            return context.AddLiveness(nameof(AzureServiceBusQueueLiveness), opt =>
            {
                opt.UseLiveness(new AzureServiceBusQueueLiveness(connectionString, queueName));
                opt.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureServiceBusTopic(this BeatPulseContext context, string connectionString, string topicName, string defaultPath = "azureservicebustopic")
        {
            return context.AddLiveness(nameof(AzureServiceBusTopicLiveness), opt =>
            {
                opt.UseLiveness(new AzureServiceBusTopicLiveness(connectionString, topicName));
                opt.UsePath(defaultPath);
            });
        }
    }
}
