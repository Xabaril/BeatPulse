﻿using BeatPulse.AzureServiceBus;
using BeatPulse.Core;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureEventHub(this BeatPulseContext context, string connectionString, string eventHubName, string name = nameof(AzureEventHubLiveness), string defaultPath = "azureeventhub")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureEventHubLiveness(connectionString, eventHubName));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureServiceBusQueue(this BeatPulseContext context, string connectionString, string queueName, string name = nameof(AzureServiceBusQueueLiveness), string defaultPath = "azureservicebusqueue")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureServiceBusQueueLiveness(connectionString, queueName));
                setup.UsePath(defaultPath);
            });
        }

        public static BeatPulseContext AddAzureServiceBusTopic(this BeatPulseContext context, string connectionString, string topicName, string name = nameof(AzureServiceBusTopicLiveness), string defaultPath = "azureservicebustopic")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new AzureServiceBusTopicLiveness(connectionString, topicName));
                setup.UsePath(defaultPath);
            });
        }
    }
}
