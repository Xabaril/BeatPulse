using BeatPulse.AzureServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddAzureEventHub(this IHealthChecksBuilder builder, string connectionString, string eventHubName, string name = nameof(AzureEventHubLiveness), string defaultPath = "azureeventhub")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureEventHubLiveness(connectionString, eventHubName));
        }

        public static IHealthChecksBuilder AddAzureServiceBusQueue(this IHealthChecksBuilder builder, string connectionString, string queueName, string name = nameof(AzureServiceBusQueueLiveness), string defaultPath = "azureservicebusqueue")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureServiceBusQueueLiveness(connectionString, queueName));
        }

        public static IHealthChecksBuilder AddAzureServiceBusTopic(this IHealthChecksBuilder builder, string connectionString, string topicName, string name = nameof(AzureServiceBusTopicLiveness), string defaultPath = "azureservicebustopic")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new AzureServiceBusTopicLiveness(connectionString, topicName));
        }
    }
}
