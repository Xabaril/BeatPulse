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
            context.Add(new AzureEventHubLiveness(connectionString, eventHubName, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureServiceBusQueue(this BeatPulseContext context, string connectionString, string queueName, string defaultPath = "azureservicebusqueue")
        {
            context.Add(new AzureServiceBusQueueLiveness(connectionString, queueName, defaultPath));
            return context;
        }

        public static BeatPulseContext AddAzureServiceBusTopic(this BeatPulseContext context, string connectionString, string topicName, string defaultPath = "azureservicebustopic")
        {
            context.Add(new AzureServiceBusTopicLiveness(connectionString, topicName, defaultPath));
            return context;
        }
    }
}
