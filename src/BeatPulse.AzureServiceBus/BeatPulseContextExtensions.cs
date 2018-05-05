using BeatPulse.AzureServiceBus;
using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAzureEventHub(this BeatPulseContext context, string connectionString, string eventHubName)
        {
            context.Add(new AzureEventHubLiveness(connectionString, eventHubName));
            return context;
        }

        public static BeatPulseContext AddAzureServiceBusQueue(this BeatPulseContext context, string connectionString, string queueName)
        {
            context.Add(new AzureServiceBusQueueLiveness(connectionString, queueName));
            return context;
        }

        public static BeatPulseContext AddAzureServiceBusTopic(this BeatPulseContext context, string connectionString, string topicName)
        {
            context.Add(new AzureServiceBusTopicLiveness(connectionString, topicName));
            return context;
        }
    }
}
